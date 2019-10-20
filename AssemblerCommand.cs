using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AOS_Lab2 {
	public partial class AssemblerCommand {
		public AssemblerCommand (string row) {
			var targetRow = ExtractContentSubstring(row);

			IsEmpty = targetRow.Length == 0;
			if (IsEmpty) return;

			ParseCommandRow(targetRow, out string name, out string[] opRands);
			this.Name = ParseName(name);

			this.commandOpRands = new List<ICommandOpRand>(opRands.Length);
			foreach (string opRandString in opRands) {
				commandOpRands.Add(ParseOpRand(opRandString));
			}
		}

		public override String ToString () {
			string result = Name.ToString().ToUpper() + " ";
			for (int i = 0; i < commandOpRands.Count; i++) {
				result += commandOpRands[i].ToString() + ((i < commandOpRands.Count) ? ", " : "" );
			}
			return result;
		}

		private ICommandOpRand ParseOpRand (string opRand) {
			if (opRand == null || opRand.Length == 0) {
				return null;
			}

			switch (opRand[0]) {
				case '$': return new ConstValue(opRand);
				case '%': return new RegName(opRand);
			}

			return null;
		}

		private OpCode ParseName (string nameString) {
			switch (nameString) {
				case "mov": return OpCode.Mov;
				case "add": return OpCode.Add;
				case "neg": return OpCode.Neg;
			}

			return OpCode.None;
		}

		private bool ParseCommandRow (string source, out string name, out string[] opRands) {
			int nameFinIndex = source.IndexOf(' ');
			nameFinIndex = (nameFinIndex == -1) ? source.Length : nameFinIndex;

			name = source.Substring(0, nameFinIndex).Trim().ToLower();

			var opRandsString = (nameFinIndex < source.Length) ? source.Substring(nameFinIndex) : "";
			opRands = opRandsString.Split(',');

			for (int i = 0; i < opRands.Length; i++) {
				opRands[i] = opRands[i].Trim().ToLower();
			}

			return name != "";
		}

		private string ExtractContentSubstring (string text) {
			int rowEndIndex = text.IndexOf('\n');
			int commentStartIndex = text.IndexOf(';');

			rowEndIndex = (rowEndIndex == -1) ? text.Length - 1 : rowEndIndex;
			commentStartIndex = (commentStartIndex == -1) ? text.Length - 1 : commentStartIndex;

			int cutIndex = Math.Min(rowEndIndex, commentStartIndex);

			return text.Substring(0, cutIndex + 1).Trim();
		}

		public enum OpCode { None, Mov, Add, Neg }

		public OpCode Name { get; private set; }
		public bool IsEmpty { get; private set; }

		public readonly List<ICommandOpRand> commandOpRands;
	}

	public partial class AssemblerCommand {
		public interface ICommandOpRand {
			string ToString ();
		}

		public class ConstValue : ICommandOpRand {
			public ConstValue (BitValue value) {
				this.Value = value;
			}

			public ConstValue (string opRand) {
				if (opRand[0] != '$') {
					throw new Exception("Wrong argument");
				}

				bool isNegative = false;
				string tarToken = opRand.Substring(1);
				if (tarToken[0] == '-') {
					tarToken = tarToken.Substring(1);
					isNegative = true;
				}

				var absValue = new BitValue(ProcessorModel.BIT_LENGTH, Convert.ToInt32(tarToken, 16));
				if (isNegative) {
					absValue.Negate();
				}

				this.Value = absValue;
			}

			public override String ToString () {
				return $"${Value.ToString()}";
			}

			public BitValue Value { get; private set; }
		}

		public class RegName : ICommandOpRand {
			public RegName (RegisterName name) {
				this.Value = name;
			}

			public RegName (string opRand) {
				if (opRand[0] != '%') {
					throw new Exception("Wrong argument");
				}
				opRand = opRand.Substring(1).Trim().ToLower();

				foreach (string regName in Enum.GetNames(typeof(RegisterName))) {
					if (opRand == regName.ToLower()) {
						this.Value = (RegisterName)Enum.Parse(typeof(RegisterName), regName);
						return;
					}
				}
			}

			public override String ToString () {
				return $"%{Value.ToString()}";
			}

			public RegisterName Value { get; private set; }
		}
	}
}
