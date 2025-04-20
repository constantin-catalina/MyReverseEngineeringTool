namespace assignment_3
{
    public interface IDiagramFormatter
    {
        string FormatClassDiagram(DiagramModel model);
        string FileExtension { get; }
    }
}