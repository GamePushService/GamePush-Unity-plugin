using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GamePush.Core
{
    public class SegmentsModule
    {
        public event Action<string> OnSegmentEnter;
        public event Action<string> OnSegmentLeave;
        
        private List<string> _segments = new();
        
        public void SetSegments(List<string> segments, List<string> enterSegments, List<string> leftSegments)
        {
            bool isNeedToSendVisitParams = 
                (_segments.Count == 0 && segments.Count > 0) || enterSegments.Count > 0 || leftSegments.Count > 0;

            if (isNeedToSendVisitParams)
            {
                CoreSDK.Analytics.SegmentsVisitParams = _segments.ToDictionary(segment => $"GP_SEGMENT_{segment}", _ => "1");
                CoreSDK.Analytics.SetVisitParams(CoreSDK.Analytics.VisitParams);
            }

            _segments = segments;

            foreach (var segment in enterSegments)
            {
                OnSegmentEnter(segment);
                
            }

            foreach (var segment in leftSegments)
            {
                OnSegmentLeave(segment);
            }
        }

        public List<string> GetList() => _segments;
        public bool Has(string tag)
        {
            return _segments.Contains(tag);
        }
    }
}
