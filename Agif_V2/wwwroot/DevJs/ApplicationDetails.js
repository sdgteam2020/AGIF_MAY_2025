$(document).on('click', '.view-pdf', function () {

    var folderName = $(this).data('folder');

    folderName = folderName.replace(/\\/g, '/');

    var fileName = $(this).data('file');

    var url = '/PdfViewer/ServeWatermarkedPdf?folderName='
        + encodeURIComponent(folderName)
        + '&fileName='
        + encodeURIComponent(fileName);

    window.open(url, '_blank');
});