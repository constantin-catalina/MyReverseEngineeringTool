using System;

namespace assignment_3
{
    public class Factory
    {
        public static IDiagramFormatter CreateFormatter(string format, DiagramOptions options)
        {
            switch (format.ToLower())
            {
                case "text":
                    return new TextFormatter(options);
                case "yuml":
                    return new YumlFormatter(options);
                case "plantuml":
                    return new PlantumlFormatter(options);
                default:
                    throw new ArgumentException($"Unknown format: {format}", nameof(format));
            }
        }
    }
}