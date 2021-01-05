using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace CustomStreetMapManager
{
    public class MapDescriptorValidation
    {
        public bool Passed { get; set; } = true;
        private List<MapDescriptorValidationMessage> issues = new List<MapDescriptorValidationMessage>();
        internal MapDescriptorValidation()
        {

        }

        public void AddProblem(int i, PropertyInfo prop, string reason)
        {
            issues.Add(new MapDescriptorValidationMessage(i, prop, reason));
        }
        public void AddProblem(PropertyInfo prop, string reason)
        {
            issues.Add(new MapDescriptorValidationMessage(prop, reason));
        }
        public string GetMessage(string seperator)
        {
            string message = "";
            for (int i = 0; i < issues.Count; i++)
            {
                var issue = issues[i];
                message += issue.ToString();
                if (i < issues.Count - 1)
                {
                    message += seperator;
                }
            }
            return message;
        }

        public void Clear()
        {
            issues.Clear();
        }

        public List<MapDescriptorValidationMessage> getIssues()
        {
            return issues;
        }
    }
}
