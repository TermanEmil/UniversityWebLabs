var loadedImgsIds = [ ];

function loadNewImg(imgNb, imgTemplate, imgsHolder) {
    var data = {
        PhotosIds: loadedImgsIds,
        RequiredImg: imgNb
    };

    var imgHolder = document.createElement('div');
    imgHolder.id = "display-img-" + imgNb;
    imgsHolder.appendChild(imgHolder);

    $.ajax({
        type: 'POST',
        url: '/PhotoWall/GetNewImg',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(data),

        success: function(result) {
            imgHolder = document.getElementById("display-img-" + result.imgNb);

            if (result.success) {
                var img = imgTemplate.content.cloneNode(true);
                img.querySelector(".grid-image").src = result.imgBase64;
                img.querySelector(".likes-count").innerHTML = result.likes;
                imgHolder.appendChild(img);

                loadedImgsIds.push(result.imgId);
            } else {
                imgsHolder.removeChild(imgHolder);
            }
        }
    });
}

function likeImg(imgId) {

}