# CLI Stack Calculator

The `clic` CLI Stack Calculator is a command-line application that implements a stack-based calculator. It leverages the [Cliffer](https://github.com/paulmooreparks/Cliffer) library to provide an interactive REPL environment with various arithmetic and stack manipulation commands.

## Usage

Run `clic` to start the interactive REPL:

```text
$ clic
clic, the CLI Stack Calculator v1.1.0.0
exit     Exit the application
help, ?  Show help and usage information
clic> + 2 3
5
```

Commands may also be executed directly from the command line:

```text
$ clic + 2 3
5
```

The stack, and any variables created, will automatically persist across sessions. The stack and the list of variables will be saved when clic is exited and reloaded when it is started.

```text
clic> push 1 2 3 4
clic> stack
4
3
2
1
clic> pop x
x = 4
clic> exit
$ clic
clic, the CLI Stack Calculator v1.1.0.0
exit     Exit the application
help, ?  Show help and usage information

clic> stack
3
2
1
clic> vars
x = 4
```

The stack will be saved to a file named `stack.txt` in the `.clic` subdirectory of the user's profile directory. Variables will be saved to a file named `vars.txt` in the same directory.

## Commands

### Stack and Variable Management

#### clear
Clear the stack and/or variables.

##### Syntax
`clear [--stack | -s] [--vars | -v]`

##### Options
- `--stack`, `-s`: Clear the stack.
- `--vars`, `-v`: Clear the variables.

##### Examples
```text
clic> push 5 10 15
clic> pop x
x = 15
clic> stack
10
5
clic> clear --stack
clic> stack
clic> vars
x = 15
clic> clear --vars
clic> vars
clic> 
```

#### del

Delete a variable.

##### Syntax
`del <variable>`

##### Parameters
- `<variable>`: The name of the variable to delete.

##### Examples
```text
clic> push 5
clic> pop x
x = 5
clic> vars
x = 5
clic> del x
clic> vars
clic> 
```

#### dup
Duplicate the top number on the stack.

##### Syntax
`dup`

##### Examples
```text
clic> push 5 10
clic> stack
10
5
clic> dup
clic> stack
10
10
5
```

#### pop
Pop a number from the stack.

##### Syntax
`pop [variable]`

##### Parameters
- `[variable]`: An optional variable to store the popped value.

##### Examples

Pop the top value from the stack and output it to the console.

```text
clic> push 5
clic> pop
5
```

Pop the top value from the stack and store it in the variable `x`.

```text
clic> push 5
clic> pop x
x = 5
clic> print x
5
```

#### push
Push one or more values onto the stack.

##### Syntax
`push <values>`

##### Parameters
- `values`: A list of one or more values to push onto the stack.

##### Examples
```text
clic> push 5
clic> stack
5
clic> push 10 15 20
clic> stack
20
15
10
5
clic> pop x
20
clic> stack
15
10
5
clic> push x
clic> stack
20
15
10
5
clic> push pi
clic> stack
3.141592653589793
20
15
10
5
```

#### load
Load stack and variables from persistent storage.

##### Syntax
`load`

#### save
Save stack and variables to persistent storage.

##### Syntax
`save`

#### stack
List all items on the stack, from top to bottom.

##### Syntax
`stack`

##### Examples
```text
clic> push 5
clic> push 10
clic> stack
10
5
```

#### swap
Swap the top two numbers on the stack.

##### Syntax
`swap`

##### Examples
```text
clic> push 5
clic> push 10
click> stack
10
5
clic> swap
clic> stack
5
10
```

#### vars
List all variables and their values.

##### Syntax
`vars`

##### Examples
```text
clic> push 5
clic> push 10
clic> pop x
x = 10
clic> pop y
y = 5
clic> vars
x = 10
y = 5
```

### Arithmetic Operations

#### **+** (Addition)
Add the top two numbers on the stack. Parameters passed to the command will be pushed onto the stack before the operation is performed.

##### Syntax
`+ [values]`

##### Examples
```text
clic> push 5
clic> push 10
clic> +
15
```

```text
clic> clear --stack
clic> + 2 4 6
12
clic> + 8
20
```

#### **-** (Subtraction)
Subtract the top number on the stack from the second number. Parameters passed to the command will be pushed onto the stack before the operation is performed.

##### Syntax
`- [values]`

##### Examples
```text
clic> push 5
clic> push 10
clic> -
5
```
   
```text
clic> clear --stack
clic> - 10 5
5
clic> - 15
-10
```

#### **\*** (Multiplication)
Multiply the top two numbers on the stack. Parameters passed to the command will be pushed onto the stack before the operation is performed.

##### Syntax
`* [values]`

##### Examples
```text
clic> push 5
clic> push 10
clic> *
50
```

```text
clic> clear --stack
clic> * 2 4
8
clic> * 8
64
```

#### **/** (Division)
Divide the second number by the top number. Parameters passed to the command will be pushed onto the stack before the operation is performed.

##### Syntax
`/ [values]`

##### Examples
```text
clic> push 5
clic> push 10
clic> /
0.5
```

```text
clic> clear --stack
clic> / 10 5
2
clic> / 15
0.13333333333333333
```

#### **mod** (Modulus)
Calculate the modulus of the second number on the stack by the top number. Parameters passed to the command will be pushed onto the stack before the operation is performed.

##### Syntax
`mod [values]`

##### Examples
```text
clic> mod 4 1
0
clic> mod 4 2
0
clic> mod 4 3
1
clic> mod 4 4
0```

#### **pow**
Raise the second number on the stack to the power of the top number. Parameters passed to the command will be pushed onto the stack before the operation is performed.

##### Syntax
`pow [values]`

##### Examples
```text
clic> pow 2 4
16
clic> pow 2
256
```

#### **sqrt**
Calculate the square root of the top number on the stack.

##### Syntax
`sqrt`

##### Examples
```text
clic> push 25
clic> sqrt
5
```

#### **log**
Calculate the natural logarithm of the top number on the stack.

##### Syntax
`log`

##### Examples
```text
clic> push 10
clic> log
2.302585092994046
```

#### **abs**
Replace the top number on the stack with its absolute value.

##### Syntax
`abs`

##### Examples
```text
clic> push -5
clic> abs
5
```

#### **sin**
Calculate the sine of the top number on the stack.

##### Syntax
`sin`

##### Examples
```text
clic> push 0
clic> sin
0
```

#### **cos**
Calculate the cosine of the top number on the stack.

##### Syntax
`cos`

##### Examples
```text
clic> push 0
clic> cos
1
```

#### **tan**
Calculate the tangent of the top number on the stack.


##### Syntax
`tan`

##### Examples
```text
clic> push 0
clic> tan
0
```

#### **cube**

Cube the top number on the stack.

##### Syntax
`cube`

##### Examples
```text
clic> push 3
clic> cube
27
```

#### **neg**
Negate the top number on the stack.

##### Syntax
`neg`

##### Examples
```text
clic> push 5
clic> neg
-5
```

#### **peek**
See the top item on the stack.

##### Syntax
`peek`

##### Examples
```text
clic> push 5
clic> peek
5
```

#### **rec**
Calculate the reciprocal of the top number on the stack.

##### Syntax
`rec`

##### Examples
```text
clic> push 5
clic> rec
0.2
```

#### **square**
Square the top number on the stack.

##### Syntax
`square`

##### Examples
```text
clic> push 5
clic> square
25
```

#### **print**
Print the value of a variable or constant.

##### Syntax
`print <variable>`
`print <constant>`

##### Parameters
- `<variable>`: The name of a variable.
- `<constant>`: The name of a constant (`e` or `pi`).

##### Examples
```text
clic> push 5
clic> pop x
x = 5
clic> print x
5
```

```text
clic> print pi
3.141592653589793
```

### Constants

Three constants are available for use in calculations: `e`, `pi`, and `tau`.

```text
clic> print e
2.718281828459045
clic> print pi
3.141592653589793
clic> print tau
6.283185307179586
clic> + pi tau
9.42477796076938
```

The characters `π` and `τ` may also be used to represent the constant values of `pi` and `tau`, respectively.

```text
clic> + π τ
9.42477796076938
```

## Macros

Macros are supported courtesy of the Cliffer library. To add macros of your own, first create an `appsettings.json` file in the `.clic` subdirectory of your user profile directory. The file should contain a `Cliffer` section with a `Macros` array. Each macro should have a `Name`, `Description`, and `Script` property. The `Script` property should contain the commands to be executed when the macro is called. Multiple commands may be provided, separated by semicolons (`;`). Arguments may be passed to the script using the `{{[arg]::n}}` syntax.

For example, the following `appsettings.json` file defines a `copy` macro that duplicates the top value on the stack and pops the top value into a variable:

```json
{
    "Cliffer": {
        "Macros": [
            {
                "Name": "copy",
                "Description": "Copy a value from top of stack into a variable, leaving the stack intact.",
                "Script": "dup;pop {{[arg]::0}}"
            }
        ]
    }
}
```

Once defined, you may use the `copy` macro as follows:

```text
clic> push 5
clic> copy x
x = 5
clic> print x
5
clic> stack
5
```

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.
