var imgViewer = document.getElementById("detailed-img-viewer");
var removeImgBtn = document.getElementById("remove-img");
var imgOwnerUsernameField = document.getElementById("img-owner-username");

function viewInDetail(img) {
    imgViewer.querySelector("#main-img").src = img.src;

    var imgId = findAncestor(img, "grid-element").id;
    imgViewer.querySelector("#target-img-id").value = imgId;
    loadAllComments(imgId);

    clearAllComments();

    loadImgOwnerData(imgId);
    removeImgBtn.hidden = true;
    imgOwnerUsernameField.innerHTML = "";

    imgViewer.hidden = false;
}

function hideDetailedImgViewer() {
    imgViewer.hidden = true;
}

function loadImgOwnerData(imgId) {
    var data = {
        imgId: imgId
    }

    $.ajax({
        type: "POST",
        url: "/PhotoWall/GetImgOwnerData/" + imgId,
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data),

        success: function(result) {
            if (result.success) {
                if (result.permissions.indexOf("Write") >= 0)
                    removeImgBtn.hidden = false;
                if (result.owner != null)
                    imgOwnerUsernameField.innerHTML = result.owner;

            } else {
                console.error(imgId, result.error);
            }
        },
        error: function(xhr, textStatus, error) {
            console.log(error);
        }
    });
}

function removeCurrentImg() {
    var imgId = imgViewer.querySelector("#target-img-id").value;
    var data = {
        imgId: imgId
    }

    $.ajax({
        type: "POST",
        url: "/PhotoWall/RemoveImg/" + imgId,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function(result) {
            if (result.success) {
                window.location.replace(result.redirUrl);
            } else {
                console.error(result.error);
            }
        },
        error: function(xhr, textStatus, error) {
            console.log(error);
        }
    });
}