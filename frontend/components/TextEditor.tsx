'use client'

import React, { useEffect, useState } from 'react'
import Editor from '@monaco-editor/react';
import {
  Menubar,
  MenubarContent,
  MenubarItem,
  MenubarMenu,
  MenubarShortcut,
  MenubarTrigger,
} from "@/components/ui/menubar"


import {
  fileOpen,
  fileSave,
} from 'browser-fs-access';

interface TextEditorProps {
  code: string;
  setCode: (code: string) => void;
  handleExecute: () => void;
}

const TextEditor = ({ code, setCode, handleExecute }: TextEditorProps) => {
  const [fileName, setFileName] = useState('');

  useEffect(() => {
    const handleKeyDown = (event: KeyboardEvent) => {
      if (event.ctrlKey && event.key.toLowerCase() === 'r') {
        event.preventDefault(); // Evita la recarga del navegador
        handleExecute();
      }
    };

    window.addEventListener('keydown', handleKeyDown);

    return () => {
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [handleExecute]);

  const openFile = async () => {
    const blob = await fileOpen({
      mimeTypes: ['image/*'],
      extensions: ['.gtl']
    });
    const text = await blob.text();
    setFileName(blob.name);
    setCode(text);
  }

  const saveFile = async () => {
    const blob = new Blob([code], { type: 'text/plain' });
    await fileSave(blob, {
      fileName: fileName || 'untitled.gtl',
      extensions: ['.gtl']
    });

  }

  return (
    <div className='flex flex-col  justify-center bg-[#131313]  w-2/3 h-full ml-3'>
      <Menubar className='bg-[#1e1e1e] text-white p-3 mb-2 border-b border-[#333]'>
        <MenubarMenu>
          <MenubarTrigger>File</MenubarTrigger>
          <MenubarContent>
            <MenubarItem onClick={openFile}>
              Open File <MenubarShortcut>⌘O</MenubarShortcut>
            </MenubarItem>
            <MenubarItem onClick={saveFile}>
              Save File <MenubarShortcut>⌘S</MenubarShortcut>
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
        <MenubarMenu>
          {/* compile */}
          <MenubarTrigger>Tools</MenubarTrigger>
          <MenubarContent>
            <MenubarItem onClick={handleExecute}>
              Execute <MenubarShortcut>⌘R</MenubarShortcut>
            </MenubarItem>
            <MenubarItem onClick={() => setCode('')}>
              Clear <MenubarShortcut>⌘K</MenubarShortcut>
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
        <MenubarMenu>
          {/* compile */}
          <MenubarTrigger>Reports</MenubarTrigger>
          <MenubarContent>
            <MenubarItem onClick={() => console.log("errors clicked")}>
              Errors
            </MenubarItem>
            <MenubarItem onClick={() => console.log("symbol clicked")}>
              Symbol Table
            </MenubarItem>
            <MenubarItem onClick={() => console.log("ast clicked")}>
              Abstract Syntax Tree
            </MenubarItem>
          </MenubarContent>
        </MenubarMenu>
      </Menubar>
      <Editor
        height="80%"
        defaultLanguage="go"
        theme='vs-dark'
        value={code}
        onChange={(value) => value && setCode(value)}

      />
    </div>


  )
}

export default TextEditor