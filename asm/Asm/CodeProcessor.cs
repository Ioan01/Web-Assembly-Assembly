using MudBlazor.Charts;
using System.Text.RegularExpressions;

namespace asm.Asm
{
	public class CodeProcessor
	{
		private static Regex startLabelRegex = new Regex(@"^(#\w+)",RegexOptions.Compiled);
        private static Regex replacementRegex = new Regex(@"(#\w+)", RegexOptions.Compiled);

        private static string[] branchingStrings = new[] { "BRA", "BEQ", "BMI", "BPL", "BLT", "BGT", "BRZ" };


        private bool IsBranching(string instruction)
        {
            return branchingStrings.Any(br => instruction.Contains(br));
        }

        private void ReplaceLabels(List<string> lines)
		{
            var labelDictionary = new Dictionary<string, int>();
            var index = 0;

			try
			{
				// add labels to dictionary
                for (var i = 0; i < lines.Count; i++)
                {
                    index = i;
                    var startMatch = startLabelRegex.Match(lines[i]);
                    if (startMatch.Success)
                    {

                        var labelName = startMatch.Groups[1].Value;
                        // if label is already defined
                        if (labelDictionary.ContainsKey(labelName))
                        {
                            MessageDisplay.AddWarning(i,
                                $"Redefinition of label {labelName} initially at line {labelDictionary[labelName]}");
                        }
                        else labelDictionary.Add(labelName, i);
                        lines[i] = lines[i].Replace(labelName, string.Empty);
                    }
                }


                for (var i = 0; i < lines.Count; i++)
                {
                    index = i;
                    var replacementMatch = replacementRegex.Match(lines[i]);
                    if (replacementMatch.Success)
                    {
                        var labelName = replacementMatch.Groups[1].Value;

                        if (labelDictionary.ContainsKey(labelName))
                        {
                            // if instruction is branching, which uses relative addresses
                            var newValue = 0;

                            if (IsBranching(lines[i]))
                            {
                                newValue = labelDictionary[labelName] - i;
                            }
                            else newValue = labelDictionary[labelName];


                            lines[i] = lines[i].Replace(labelName, newValue.ToString());
                        }
                        else
                        {
                            MessageDisplay.AddError(i, $"Label {labelName} used but not defined.");
                        }
                    }

                }

            }
            catch (Exception e)
			{
				MessageDisplay.AddError(index,"Error parsing label.");
                throw;
            }

        }

		
		public List<IntermediaryInstruction> ProcessInstructions(string code)
		{
			if (code.Length == 0)
			{
				return null;
			}
			List<string> lines;
			lines = code.Contains('\n') ? code.Split('\n').ToList() : new List<string>(new []{code});
			
			ValidateInstructions(lines);
			
            ReplaceLabels(lines);

            lines = lines.Where(line => !string.IsNullOrEmpty(line)).ToList();

			return new List<IntermediaryInstruction>(lines.Select(line =>
			{
				line = line.Trim();
				
				var instruction = new IntermediaryInstruction();
				if (!line.Contains(' '))
				{
					instruction.Alias = line;
					return instruction;
				}
				
				instruction.Alias = line.Substring(0, line.IndexOf(' '));
				line = line.Substring(line.IndexOf(' ') + 1);

				while (line.Length > 0)
				{
					string argument = line.Substring(0, line.IndexOf(',') != -1 ? line.IndexOf(',') : line.Length);
					line = line.Substring(line.IndexOf(',') != -1 ? line.IndexOf(',') + 1 : line.Length);
					
					instruction.Arguments.Add(new Argument(argument));
				}
	
				return instruction;
			})).Where(line=>!string.IsNullOrEmpty(line.Alias)).ToList();


		}

		private void ValidateInstructions(List<string> strings)
		{
			
			
			
		}
	}
}
