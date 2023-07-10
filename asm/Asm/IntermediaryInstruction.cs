namespace asm.Asm;

public class IntermediaryInstruction
{
    public string Alias { get; set; }
    public List<Argument> Arguments { get; set; } = new();
}