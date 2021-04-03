import * as path from 'https://deno.land/std/path/mod.ts';

const dllSrcPaths = [
    String.raw`C:\__work__\kmb\210322-net5\DataSource\bin\Release\net5.0\DataSource.dll`,
    String.raw`C:\__work__\kmb\210322-net5\DataSource\bin\Release\net5.0\Resources.dll`,
    String.raw`C:\__work__\kmb\210322-net5\FileDataSource\bin\Release\net5.0\FileDataSource.dll`,
    String.raw`C:\__work__\kmb\210322-net5\DBDataSource\bin\Release\net5.0\DBDataSource.dll`,
];

const destDir = String.raw`C:\__work__\dotnet\Electricity\dll`;

for (const dllPath of dllSrcPaths) {
    const destPath = path.join(destDir, path.basename(dllPath));
    console.log(`Copying "${dllPath}" to "${destPath}"`);
    Deno.copyFileSync(dllPath, destPath);
}
