using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumble.Client.Extensions;
using Tumble.Core;
using Tumble.Core.Helpers;
using Tumble.Core.Providers;
using Tumble.Handlers.Proxy.Contexts;

namespace Tumble.Handlers.Resiliency
{
    public class CircuitBreakerConfiguration
    {
        public TimeSpan CircultSlidingDuration { get; set; }
        public int ClosedToOpenFailureTrigger { get; set; }
        public int OpenToClosedFailureTrigger { get; set; }
        public TimeSpan OpenToHalfOpenTimeout { get; set; }
    }

    public enum CircuitBreakerEnum { OpenState };

    public class CircuitBreakHandler : IPipelineHandler<IHttpRequestResponseContext>
    {
        
        public IDateTimeProvider _dateTimeProvider;
        private LockableObject<CircuitState> _circuitState;

        public CircuitBreakHandler(IDateTimeProvider dateTimeProvider, CircuitBreakerConfiguration configuration)
        {
            _dateTimeProvider = dateTimeProvider;
            _circuitState = new LockableObject<CircuitState>()
            {
                Object = new CircuitState(
                    _dateTimeProvider,
                    configuration.CircultSlidingDuration,
                    configuration.ClosedToOpenFailureTrigger,
                    configuration.OpenToClosedFailureTrigger,
                    configuration.OpenToHalfOpenTimeout)
            };
        }

        public async Task InvokeAsync(PipelineDelegate next, IHttpRequestResponseContext context)
        {
            //context.Remove(CircuitBreakerEnum.OpenState);

            CircuitState.State currentState = CircuitState.State.Closed;
            _circuitState.Invoke(x =>
                currentState = x.GetState());

            var activeStates = new[] { CircuitState.State.Closed, CircuitState.State.Half_Open };
            if (activeStates.Any(x => x == currentState))
            {
                await next.Invoke();

                //if (!context.GetFirst(out HttpResponseMessage httpResponseMessage))
                //    throw new PipelineDependencyException<HttpResponseMessage>(this);

                _circuitState.Invoke(x =>
                    x.Update(!context.HttpResponseMessage.IsTransientFailure()));
            }
            else
            {
                //context.Add(CircuitBreakerEnum.OpenState);
            }
        }        

        internal class CircuitState
        {
            private IDateTimeProvider _dateTimeProvider;

            public CircuitState(IDateTimeProvider dateTimeProvider,
                TimeSpan circultSlidingDuration,
                int ClosedToOpenFailureTrigger,
                int OpenToClosedFailureTrigger,
                TimeSpan OpenToHalfOpenTimeout)
            {
                _dateTimeProvider = dateTimeProvider;
                _circultSlidingDuration = circultSlidingDuration;
                _closedToOpenFailureTrigger = ClosedToOpenFailureTrigger;
                _openToClosedFailureTrigger = OpenToClosedFailureTrigger;
                _openToHalfOpenTimeout = OpenToHalfOpenTimeout;

                if (ClosedToOpenFailureTrigger <= 0)
                    throw new ArgumentException("ClosedtoOpenFailureTrigger must be greater than 0");
                if (OpenToClosedFailureTrigger <= 0)
                    throw new ArgumentException("OpenToClosedFailureTrigger must be greater than 0");
                if (ClosedToOpenFailureTrigger <= OpenToClosedFailureTrigger)
                    throw new ArgumentException("ClosedToOpenFailureTrigger must be greater than OpenToClosedFailureTrigger");
            }


            private IList<DateTime> _failures;
            private IList<DateTime> GetFailuresWithinThreshold(TimeSpan thresHold) =>
                (_failures ?? new List<DateTime>())
                    .Where(x => x > _dateTimeProvider.UtcNow() - thresHold)
                    .ToList();

            private TimeSpan _circultSlidingDuration;

            private int _closedToOpenFailureTrigger;
            private bool FailuresExceedClosedToOpenThreshold(int failureCount) =>
                failureCount >= _closedToOpenFailureTrigger;

            private int _openToClosedFailureTrigger;
            private bool FailuresBelowOpenToClosedThreshold(int failureCount) =>
                failureCount <= _openToClosedFailureTrigger;

            public enum State { Closed, Open, Half_Open };
            private State _state = State.Closed;

            private TimeSpan _openToHalfOpenTimeout;
            private DateTime _lastHalfOpen;
            private bool HalfOpenTimeout() =>
                _lastHalfOpen + _openToHalfOpenTimeout >= _dateTimeProvider.UtcNow();

            public State GetState()
            {
                _failures = GetFailuresWithinThreshold(_circultSlidingDuration);
                if ((_state == State.Closed) && (_failures.Count >= _closedToOpenFailureTrigger))
                {
                    _state = State.Open;
                    _lastHalfOpen = _dateTimeProvider.UtcNow();
                }
                else
                if ((_state == State.Open) && (_failures.Count <= _openToClosedFailureTrigger))
                {
                    _state = State.Closed;
                }
                else
                if ((_state == State.Open) && (HalfOpenTimeout()))
                {
                    _lastHalfOpen = _dateTimeProvider.UtcNow();
                    return State.Half_Open;
                }
                return _state;
            }

            public void Update(bool success)
            {
                switch (success)
                {
                    case true:
                        _failures.Clear();
                        break;
                    case false:
                        _failures.Add(_dateTimeProvider.UtcNow());
                        break;
                }
                if (!success)
                    _failures.Add(_dateTimeProvider.UtcNow());


            }

        }

    }
}
