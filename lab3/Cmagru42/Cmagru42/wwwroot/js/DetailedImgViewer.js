var imgViewer = document.getElementById("detailed-img-viewer");

function viewInDetail(img) {
    imgViewer.hidden = false;
    imgViewer.querySelector("#main-img").src = img.src;

    var imgId = findAncestor(img, "grid-element").id;
    imgViewer.querySelector("#target-img-id").value = imgId;
    loadAllComments(imgId);

    clearAllComments();
}

function hideDetailedImgViewer() {
    imgViewer.hidden = true;
}

function loadImgOwnerPermissions(imgId) {
    var data = {
        imgId: imgId
    }

    $.ajax({
        type: "POST",
        url: "/PhotoWall/GetImgOwnerPermissions",
        dataType: "json",
        contentType: "text",
        data: JSON.stringify(data),

        success: function(result) {
            console.log(result);
        },
        error: function(xhr, textStatus, error) {
        }
    });
}