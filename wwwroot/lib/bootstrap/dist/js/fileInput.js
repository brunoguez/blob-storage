$(async () => {
    const $formFileMultiple = $("#formFileMultiple")
    $formFileMultiple.on('change', async (e) => {
        const files = $formFileMultiple.prop('files');

        for (const file of files) {

            const formData = new FormData();
            formData.append('file', file);

            try {
                await axios({
                    method: 'POST',
                    url: "/api/upload",
                    headers: {
                        'Content-Type': 'multipart/form-data'
                    },
                    data: formData,
                });
            } catch (erro) {
                console.error({ e, erro, files });
            }
        }

        location.reload();

    });

    configureTable();
});

async function getFileList() {
    try {
        const res = await axios({
            method: 'GET',
            url: "/api/list",
        }).then(({ data }) => data);
        return res;
    } catch (e) {
        console.error(e);
        return [];
    }
}
async function configureTable() {
    const $tableBody = $('#tableBody');
    const filesServer = await getFileList();
    let count = 0;
    for (const fileName of filesServer) {
        count++;
        const $tr = $('<tr>');
        $('<th>').attr('scope', 'row').text(count).appendTo($tr);
        $('<td>').text(fileName).appendTo($tr);
        $('<td>')
            .addClass("text-center")
            .append(
                $('<i>')
                    .addClass("fa fa-trash")
                    .css('cursor', 'pointer')
                    .on('click', async () => {
                        await deleteFile(fileName);
                        location.reload();
                    })
            ).appendTo($tr);
        $('<td>')
            .addClass("text-center")
            .append(
                $('<i>')
                    .addClass("fa fa-download")
                    .css('cursor', 'pointer')
                    .on('click', () => downloadFile(fileName))
            ).appendTo($tr);
        $tr.appendTo($tableBody)
    }

}
async function deleteFile(fileName) {
    try {
        await axios({
            method: 'DELETE',
            url: "/api/" + fileName,
        });
    } catch (e) {
        throw e;
    }
}
async function downloadFile(fileName) {
    try {
        await axios({
            method: 'GET',
            url: "/api/download/" + fileName,
            responseType: 'blob'
        }).then((response) => {
            const blob = response.data;
            const link = document.createElement('a');
            const url = window.URL.createObjectURL(blob);
            link.href = url;
            link.download = fileName;
            link.click();
            window.URL.revokeObjectURL(url);
        });
    } catch (e) {
        throw e;
    }
}