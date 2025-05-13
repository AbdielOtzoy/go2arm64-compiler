# 🧬 Compilador con ANTLR4 + C# (.NET) + Next.js

Este proyecto es un compilador educativo e interactivo que combina la potencia de **ANTLR4** para el análisis léxico y sintáctico, con la robustez de **C# y .NET** para el procesamiento backend, y una interfaz moderna hecha con **Next.js**.

> 📌 Ideal para quienes desean entender cómo funciona un compilador desde su gramática hasta su ejecución, todo en un entorno visual y accesible desde el navegador.

---

## 🚀 ¿Qué hace este proyecto?

- Analiza código fuente escrito por el usuario.
- Genera árboles de sintaxis abstracta (AST) utilizando ANTLR4.
- Reporta errores léxicos y sintácticos.
- Muestra el resultado en una interfaz web responsiva.
- Permite experimentar y modificar reglas gramaticales.

---

## ⚙️ ¿Cómo ejecutarlo?

### 1. Clona el repositorio
```bash
git clone https://github.com/tu-usuario/nombre-del-repo.git
cd nombre-del-repo
```

### 2. Ejecuta el Backend (.NET + ANTLR4)
```bash
dotnet watch run
```

### 3. Ejecuta el Frontend (Next.js con pnpm)
```bash
cd ../frontend
pnpm i
pnpm run dev
```
### 🧠 Requisitos Previos
.NET SDK

Node.js y pnpm

ANTLR4 (opcional si ya están generados los analizadores)


### 🧠 ¿Por qué ARM64?
ARM64 es una arquitectura ampliamente usada en procesadores móviles, dispositivos embebidos y ahora también en servidores y computadoras modernas (como Apple Silicon). Este compilador ayuda a entender cómo se traduce un lenguaje como Go a instrucciones reales que puede ejecutar un procesador ARM moderno.
