#!/bin/bash

# Ensamblar el código fuente
aarch64-linux-gnu-as -mcpu=cortex-a57 hello-world.s -o hello-world.o
if [ $? -ne 0 ]; then
    echo "Error: Fallo en la ensamblación."
    exit 1
fi

# Vincular el archivo objeto
aarch64-linux-gnu-ld hello-world.o -o hello-world
if [ $? -ne 0 ]; then
    echo "Error: Fallo en la vinculación."
    exit 1
fi

# Ejecutar con QEMU
echo "Ejecutando en QEMU..."
qemu-aarch64 ./hello-world
