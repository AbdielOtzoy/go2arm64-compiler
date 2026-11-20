# go2arm64 — a compiler from a Go subset to ARM64 assembly

A compiler for **GoLight**, a subset of Go, that emits AArch64 (ARM64) assembly. The assembly is built with GNU binutils and runs on QEMU. A Next.js web editor sends code to a C#/.NET API that parses it with ANTLR4 and returns the generated assembly.

Course project for *Organización de Lenguajes y Compiladores 2* (Universidad de San Carlos de Guatemala, 1st semester 2025). Individual work. It continues my [GoLight interpreter](https://github.com/AbdielOtzoy/OLC2_Proyecto1_202300350), which runs the same language with a tree-walking interpreter.

## How it works

```text
Next.js editor ──POST /compile──► ASP.NET Core API
                                     │
                                     ├─ ANTLR4 lexer + parser (Language.g4) ──► parse tree
                                     ├─ FrameVisitor   computes stack frames for functions
                                     └─ CompilerVisitor walks the tree ──► arm/Generator ──► .s file
                                                                              │
                                     aarch64-linux-gnu-as / ld ──► qemu-aarch64 (see backend/output/run.sh)
```

- **Grammar:** [`backend/grammars/Language.g4`](backend/grammars/Language.g4). It covers variables and `:=`, `int`/`float64`/`string`/`bool`/`rune`, `if`/`switch`/`for`/`for range`, `break`/`continue`, functions, slices and matrices, structs, and built-ins such as `append`, `len`, `strconv.Atoi`, `strings.Join` and `slices.Index`.
- **Code generation:** [`backend/compiler`](backend/compiler). It manages the stack and registers (`arm/Registers.cs`), emits instructions (`arm/Generator.cs`), and includes a small runtime written in assembly that prints integers, floats, booleans, runes, strings and arrays (`arm/Std.cs`). Program output goes through Linux syscalls (`svc #0`).
- **Errors:** the API reports lexical, syntax and semantic errors with line and column.
- **AST view:** the editor can draw the parse tree as SVG through ANTLR Lab.

## Run it

Requirements: .NET 9 SDK, Node.js with pnpm, and `aarch64-linux-gnu` binutils plus `qemu-user` to run the output.

```bash
# API
cd backend && dotnet watch run

# Web editor
cd frontend && pnpm install && pnpm dev
```

To run a generated program, save the assembly as `backend/output/hello-world.s` and run `backend/output/run.sh`.

## Docs (Spanish)

- [Technical manual](docs/ManualTecnicoGoLigth.pdf)
- [User manual](docs/ManualUsuarioGoLigth.pdf)
- [Grammar](docs/gramaticaANTLR4.pdf)
