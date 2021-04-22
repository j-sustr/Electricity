import * as path from 'https://deno.land/std/path/mod.ts';

const dllPaths = [
    String.raw`C:\__work__\kmb\datasource-net5\DataSource\bin\Release\net5.0\DataSource.dll`,
    String.raw`C:\__work__\kmb\datasource-net5\DataSource\bin\Release\net5.0\Resources.dll`,
    String.raw`C:\__work__\kmb\datasource-net5\FileDataSource\bin\Release\net5.0\FileDataSource.dll`,
    String.raw`C:\__work__\kmb\datasource-net5\DBDataSource\bin\Release\net5.0\DBDataSource.dll`,
];

const pdbPaths = [
    String.raw`C:\__work__\kmb\datasource-net5\DataSource\bin\Release\net5.0\DataSource.pdb`,
    String.raw`C:\__work__\kmb\datasource-net5\DataSource\bin\Release\net5.0\Resources.pdb`,
    String.raw`C:\__work__\kmb\datasource-net5\FileDataSource\bin\Release\net5.0\FileDataSource.pdb`,
    String.raw`C:\__work__\kmb\datasource-net5\DBDataSource\bin\Release\net5.0\DBDataSource.pdb`,
];

const srcPaths = [...dllPaths, ...pdbPaths];

const destDir = String.raw`C:\__work__\dotnet\Electricity\dll`;

for (const srcPath of srcPaths) {
    const destPath = path.join(destDir, path.basename(srcPath));
    console.log(`Copying "${srcPath}" to "${destPath}"`);
    Deno.copyFileSync(srcPath, destPath);
}
