
export function initializeLanguage() {
	console.log("Adding language");
	console.log(monaco)
	monaco.languages.register({ id: "mySpecialLanguage" });

	// Register a tokens provider for the language
	monaco.languages.setMonarchTokensProvider("mySpecialLanguage", {
		tokenizer: {
			root: [
				[/\[error.*/, "custom-error"],
				[/\[notice.*/, "custom-notice"],
				[/\[info.*/, "custom-info"],
				[/\[[a-zA-Z 0-9:]+\]/, "custom-date"],
				[/(MOV|BEQ|OUT|SUB|BRA|LDR|STR|HLT|JMS|PSH|POP|RET|CMP|BRZ|BMI|BPL|BGT|BLT|ADD|MUL|DIV|MOD|AND|OR|XOR|RSHIFT|LSHIFT)/,
					"instructions"],
				[/#[\w\d]+/, "labels"],
				[/R[0-7]/, "registers"],
				[/(0x[\da-fA-F]+)|('\\?\w')/,"values"]
			],
		},
	});

	monaco.editor.defineTheme("myCoolTheme", {
		base: "vs-dark",
		inherit: true,
		rules: [
			{ token: "custom-info", foreground: "808080" },
			{ token: "custom-error", foreground: "ff0000", fontStyle: "bold" },
			{ token: "custom-notice", foreground: "FFA500" },
			{ token: "custom-date", foreground: "008800" },
			{ token: "instructions", foreground: "008800", fontStyle: "bold" },
			{ token: "labels", foreground: "FFA500" },
			{ token: "registers", foreground: "33b2ff" },
			{ token: "values", foreground:"764240" }
		],
		colors: {
		},
	});


	monaco.languages.registerCompletionItemProvider("mySpecialLanguage", {
		provideCompletionItems: (model, position) => {
			var word = model.getWordUntilPosition(position);
			var range = {
				startLineNumber: position.lineNumber,
				endLineNumber: position.lineNumber,
				startColumn: word.startColumn,
				endColumn: word.endColumn,
			};
			var suggestions = [
				{
					label: "Instructions",
					kind: monaco.languages.CompletionItemKind.Keyword,
					insertText: "testing(${1:condition})",
					insertTextRules:
						monaco.languages.CompletionItemInsertTextRule
							.InsertAsSnippet,
					range: range,
				},
				{
					label: "ifelse",
					kind: monaco.languages.CompletionItemKind.Snippet,
					insertText: [
						"if (${1:condition}) {",
						"\t$0",
						"} else {",
						"\t",
						"}",
					].join("\n"),
					insertTextRules:
						monaco.languages.CompletionItemInsertTextRule
							.InsertAsSnippet,
					documentation: "If-Else Statement",
					range: range,
				},
			];
			return { suggestions: suggestions };
		},
	});


}



