const dllPath1 = String.raw`C:\__work__\kmb\210322-net5\DataSource\bin\Release\net5.0\DataSource.dll`;
const dllPath2 = String.raw`C:\__work__\kmb\210322-net5\FileDataSource\bin\Release\net5.0\DataSource.dll`;

const bytes1 = Deno.readFileSync(dllPath1);
const bytes2 = Deno.readFileSync(dllPath2);

console.log(bytes1);
console.log(bytes2);
