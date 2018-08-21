using System;
using System.Collections.Generic;
using System.Text;

namespace Tumble.Core
{
    public class PipelineContextCollection
    {
        private Dictionary<string, PipelineContext> _pipelines = new Dictionary<string, PipelineContext>();

        public bool Add(string name, PipelineContext pipelineContext) =>
            _pipelines.TryAdd(name, pipelineContext);        

        public PipelineContext Get(string name) =>
            _pipelines.TryGetValue(name, out PipelineContext pipelineContext)
                ? pipelineContext
                : null;

        public bool Get(string name, out PipelineContext pipelineContext)
        {
            pipelineContext = Get(name);
            return pipelineContext != null;
        }
    }
}
