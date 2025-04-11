'use client'

import TextEditor from "@/components/TextEditor";
import { useState } from "react";
import { toast } from "sonner"

import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"

import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import { Button } from "@/components/ui/button";
import { Copy } from "lucide-react";

type Symbol = {
  id: string;
  type: string;
  kind: string;
  line: string;
  column: string;
}

export default function Home() {
  const [code, setCode] = useState('');
  const [output, setOutput] = useState('');
  const [jsonTable, setJsonTable] = useState<Symbol[]>([]);
  const [error, setError] = useState('');
  const [svgASt, setSvgASt] = useState('');

  const handleExecute = async () => {
    try {
      const res = await fetch('http://localhost:5215/compile', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ code })
      });
      
      console.log(res);
      const data = await res.json();
      console.log(data);

      if(!res.ok) {
        toast.error(data.message || 'Something went wrong');
        setError(data.message || 'Something went wrong');
        throw new Error(data.message || 'Something went wrong');
      }

      setOutput(data.result);
      // convert data.table to jsonTable
      // const parsedTable = JSON.parse(data.symbols);
      // console.log(parsedTable);
      // setJsonTable(parsedTable);
      

    } catch (error) {
      console.log(error);
      setOutput('');
      
    }
  }

  const handleAST = async () => {
    try {
      const res = await fetch('http://localhost:5215/compile/ast', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({ code })
      });
      
      // recibir el ast en formato svg
      console.log(res);
      const data = await res.json();
      console.log(data.svgtree);
      setSvgASt(data.svgtree);
      

    } catch (error) {
      console.log(error);
      setOutput('');
      
    }
  }
  

  const handleCopyOutput = () => {
    navigator.clipboard.writeText(output)
      .then(() => toast.success("Output copied to clipboard!"))
      .catch(() => toast.error("Failed to copy output"));
  };

  return (
    <div>
      <div className="flex h-screen bg-[#131313] align-center justify-center gap-2">

      <TextEditor 
        code={code}
        setCode={setCode}
        handleExecute={handleExecute}
      />

      <div className='bg-[#1e1e1e] text-white p-4 h-[80vh] flex flex-col gap-4 w-1/3 mt-[91px] mr-3 overflow-auto'>
        <div className="flex justify-between">
          <h1 className="text-2xl">Output</h1>
          <Button onClick={handleCopyOutput}>
            <Copy className="h-4 w-4" />
          </Button>
        </div>
        <pre className="text-sm">{output}</pre>
      </div >

      </div>
      <div className="flex space-x-3 justify-center bg-[#131313] text-white p-4 h-[10vh] w-full">
      <Dialog>
      <DialogTrigger>Symbols table</DialogTrigger>
      <DialogContent className="w-full overflow-auto h-[50vh]">
        <DialogHeader>
          <DialogTitle>Symbols Table</DialogTitle>
          <DialogDescription>
          

          </DialogDescription>
        </DialogHeader>
        <Table className="w-full overflow-auto">
          <TableCaption>Table of symbols</TableCaption>
          <TableHeader>
            <TableRow>
              <TableHead className="w-[100px]">ID</TableHead>
              <TableHead>Tipo símbolo</TableHead>
              <TableHead>Tipo dato</TableHead>
              <TableHead>Linea</TableHead>
              <TableHead>Columna</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody className="divide-y divide-gray-200">
            {jsonTable.map((row) => (
              <TableRow key={Math.random()}>
                <TableCell>{row.id}</TableCell>
                <TableCell>{row.type}</TableCell>
                <TableCell>{row.kind}</TableCell>
                <TableCell>{row.line}</TableCell>
                <TableCell>{row.column}</TableCell>

              </TableRow>
            ))}
          </TableBody>
        </Table>
      </DialogContent>
    </Dialog>
    <Dialog>
      <DialogTrigger>Errors</DialogTrigger>
      <DialogContent className="w-full overflow-auto h-[50vh]">
        <DialogHeader>
          <DialogTitle>Symbols Table</DialogTitle>
          <DialogDescription>
          

          </DialogDescription>
        </DialogHeader>
        <Table className="w-full overflow-auto">
          <TableCaption>Table of errors</TableCaption>
          <TableHeader>
            <TableRow>
              <TableHead className="w-[100px]">Error</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody className="divide-y divide-gray-200">
            
              <TableRow>
                <TableCell>{error}</TableCell>
              </TableRow>
          </TableBody>
        </Table>
      </DialogContent>
    </Dialog>
    <Button onClick={handleAST}>ast</Button>
    <Button onClick={() => {
        setJsonTable([]);
        setSvgASt('');
        setError('');
    }}>Clear AST</Button>
      </div>
      <div className="w-full h-full overflow-auto">
        <div dangerouslySetInnerHTML={{__html: svgASt}} />
      </div>
    </div>
  );
}