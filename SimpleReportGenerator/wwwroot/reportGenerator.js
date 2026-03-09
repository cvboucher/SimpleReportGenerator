export function downloadFileFromStream(fileName, contentStreamReference) {
    contentStreamReference.arrayBuffer().then(buffer => {
        const blob = new Blob([buffer]);
        const url = URL.createObjectURL(blob);
        const anchorElement = document.createElement('a');
        anchorElement.href = url;
        anchorElement.download = fileName ?? 'report.xlsx';
        anchorElement.click();
        URL.revokeObjectURL(url);
    });
}
