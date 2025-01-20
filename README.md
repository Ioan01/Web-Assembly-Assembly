# asm
# Little Man Computer-like Emulator
### Inspired by  [Peter Higginson's Online Emulator](https://peterhigginson.co.uk/RISC/)
### Follow-up with Blazor Frontend of [MediumMan Computer Emulator](https://github.com/Ioan01/MediumManComputerEmulator)
##
##
## Specifications:
- 32-bit Operation Size (6 bit opcode and 26 bit arguments )
- 4096-Word (16384 byte) Memory Size
- 8 General Purpose 32-bit Registers
- 32-bit Link Register
- 32-bit Stack Pointer
- 44 instructions
- 4 1-bit Flags (Zero, Negative ,Carry ,Overflow )

## Syntax
- General syntax :  <operation name> [<arg1>],[<arg2>]
- Labels : #LabelName <instruction> <arguments> 
        BRANCH/JUMP #LabelName
- Immediate values : <operation name>, 0-255

## Workflow
- Program is loaded line by line into the Emulator. 
- The emulator will first parse labels. For example if it encounters the label #LABELNAME at line 1 it will add it to an internal dictionary as [#LABELNAME,1] and will replace each further occurance of #LABELNAME with 1.
- Then, the emulator will decode each instruction by matching the string with a regex to find out 
-- the instruction
-- the arguments
- Depending on the operation name, it will instantiate an Instruction child class and pass the arguments to the constructor. For example, if STR is found, a StoreInstruction will be instantiated.
- Then, the operation will parse the arguments and return a 32-bit integer representing the instruction bits.
- These 16 bits are created by : (6-bit opcode  << 26) | (26 bits arguments)
- For example for the instruction STR R0,127
    The opcode of STR is 0x3 << 26 
    Seeing as the first argument is register 0, the first bit from the argument bits will be 0
    Since the second argument contains an immediate value, the next bit will be 1 to specify that an 8-bit immediate value is used. Therefore, the last 8 bits will represent our immedaite value.
    Hence, we have 000 011 0000000000 (opcode) | 000 000 0 000000000 (register selector) | 000 000 0 1(bit that shows that the following 8 bits are an immediate value)01111111(8-bit immediate value)
- Then, the 16-bit number is stored in the virtual memory at increasing addresses.
- Then, we start the emulator, which will 
    - Read the the 32-bit word at memory[ProgramCounter]
    - Increment Program Counter
    - Insantiate an Instruction child class based on the first 6 bits, and pass the following 10 bits as arguments
    - Execute the Instruction passing the emulator as parameter to the Instruction
   


## Instruction Structure

### 6-bit opcode and 26-bit arguments

| o 	| o 	| o 	| o 	| o 	| o 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	|  | a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 	| a 
|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|  |---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---	|---


## Instructions
#
#
#
| NR  | Alias | Opcode | Arguments | Name                                                   |
| --- | ----- | ------ | --------- | ------------------------------------------------------ |
| 0   | INP   | 000000 | "[7:0] -> port, [10:8] -> register selector" | Input from port                                        |
| 1   | OUT   | 000001 | "[7:0] -> port, [10:8] -> register selector" | Output to port                                         |
| 2   | LDR   | 000010 | "[25:23] -> destination register selector, [22:0] -> 22-bit immediate address" | Load register immediate address (LDR Rd, 65535)       |
| 3   | LDR   | 000011 | "[0:2] -> address register selector, [3:5] -> destination register selector" | Load register (register indirect addressing, LDR Rd, Ra) |
| 4   | LDR   | 000100 | "[25:23] -> destination register selector, [22:3] -> base address, [2:0] -> offset register selector" | Load register (indirect register addressing + 16-bit immediate, LDR R2, 65535, offset R4) |
| 5   | STR   | 000101 | "[25:23] -> source register selector, [22:0] -> 22-bit immediate address" | Store register immediate address (STR Rd, 65535)     |
| 6   | STR   | 000110 | "[0:2] -> address register selector, [3:5] -> source register selector" | Store register (register indirect addressing, STR Rd, Ra) |
| 7   | STR   | 000111 | "[25:23] -> source register selector, [22:3] -> base address, [2:0] -> offset register selector" | Store register (indirect register addressing + 16-bit immediate, STR Rx, 65535, offset Rx) |
| 8   | HLT   | 001000 | "[25:0] unused" | Halt                                                   |
| 9   | JMS   | 001001 | "[25:0] immediate address" | Jump to immediate address                              |
| 10  | PSH   | 001010 | "[2:0] -> source register selector" | Push register to stack                                 |
| 11  | PSH   | 001011 | "[25:0] -> immediate value" | Push immediate value to stack                          |
| 12  | POP   | 001100 | "[2:0] -> destination register selector" | Pop to register                                        |
| 13  | RET   | 001101 | "[25:0] unused" | Return PC to LR                                        |
| 14  | CMP   | 001110 | "[25:23] -> register selector, [22:0] -> immediate value" | Compare register with immediate value (CMP R0, 2)    |
| 15  | CMP   | 001111 | "[2:0] -> R1 register selector, [3:5] -> R2 register selector" | Compare register with register (CMP R0, R2)         |
| 16  | BRA   | 010000 | "[25:0] -> relative address" | Relative branch always                                 |
| 17  | BEQ   | 010001 | "[25:0] -> relative address" | Relative branch if equal                               |
| 18  | BRZ   | 010010 | "[25:0] -> relative address" | Relative branch if zero                                |
| 19  | BMI   | 010011 | "[25:0] -> relative address" | Relative branch if minus                               |
| 20  | BPL   | 010100 | "[25:0] -> relative address" | Relative branch if positive                            |
| 21  | BGT   | 010101 | "[25:0] -> relative address" | Relative branch if greater than                        |
| 22  | BLT   | 010110 | "[25:0] -> relative address" | Relative branch if less than                           |
| 23  | ADD   | 010111 | "Add immediate value to register" | ADD R0, 2134                                           |
| 24  | ADD   | 011000 | "Add register to register" | ADD R0, R1                                             |
| 25  | SUB   | 011001 | "Subtract immediate value to register" | Subtract immediate value from register                 |
| 26  | SUB   | 011010 | "Subtract register to register" | Subtract register from register                        |
| 27  | MUL   | 011011 | "Multiply immediate value with register" | Multiply immediate value with register                  |
| 28  | MUL   | 011100 | "Multiply register with register" | Multiply register with register                         |
| 29  | DIV   | 011101 | "Divide immediate value with register" | Divide immediate value with register                    |
| 30  | DIV   | 011110 | "Divide register with register" | Divide register with register                           |
| 31  | MOD   | 011111 | "Modulo immediate value with register" | Modulo immediate value with register                    |
| 32  | MOD   | 100000 | "Modulo register with register" | Modulo register with register                           |
| 33  | AND   | 100001 | "Bitwise AND immediate value with register" | Bitwise AND immediate value with register               |
| 34  | AND   | 100010 | "Bitwise AND register with register" | Bitwise AND register with register                      |
| 35  | OR    | 100011 | "Bitwise OR immediate value with register" | Bitwise OR immediate value with register                |
| 36  | OR    | 100100 | "Bitwise OR register with register" | Bitwise OR register with register                       |
| 37  | XOR   | 100101 | "Bitwise XOR immediate value with register" | Bitwise XOR immediate value with register               |
| 38  | XOR   | 100110 | "Bitwise XOR register with register" | Bitwise XOR register with register                      |
| 39  | RSHIFT| 100111 | "Rshift register with immediate value positions" | Rshift register with immediate value positions         |
| 40  | RSHIFT| 101000 | "Rshift register with register value positions" | Rshift register with register value positions           |
| 41  | LSHIFT| 101001 | "Lshift register with immediate value positions" | Lshift register with immediate value positions         |
| 42  | LSHIFT| 101010 | "Lshift register with register value positions" | Lshift register with register value positions           |
| 43  | MOV   | 101011 | "Move immediate value in register" | Move immediate value in register                        |
| 44  | MOV   | 101100 | "Move register in register" | Move register in register                               |
                                        | move imeediate value into register                                                                                                                                                              |
