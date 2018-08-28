using System;
using System.Collections.Generic;
using System.Text;

namespace Tumble.Core
{
    public class PipelineRequestCollection
    {
        private Dictionary<string, PipelineRequest> _pipelines = new Dictionary<string, PipelineRequest>();

        public bool Add(string name, PipelineRequest pipelineRequest) =>
            _pipelines.TryAdd(name, pipelineRequest);

        public PipelineRequest Get(string name) =>
            _pipelines.TryGetValue(name, out PipelineRequest pipelineRequest)
                ? pipelineRequest
                : null;

        public bool Get(string name, out PipelineRequest pipelineRequest)
        {
            pipelineRequest = Get(name);
            return pipelineRequest != null;
        }
    }
}
