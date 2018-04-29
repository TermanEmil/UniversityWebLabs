var loadedImgsIds = [ ];
var imgTemplate;
var imgsHolder;
var deltaImgsToLoad = 5;
var moreImgsBtn;
var loadingImgsMsgP;
var requestStatuses = {};

function loadNewImg(imgNb) {
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

            requestStatuses[result.imgNb] = true;
            if (loadImgsRequestsFinished())
                onLoadImgsEnd();
        },
        error: function(xhr, textStatus, error) {
            requestStatuses[result.imgNb] = true;
            if (loadImgsRequestsFinished())
                onLoadImgsEnd();
        }
    });
}

function initPhotoWall()
{
    imgTemplate = document.getElementById("grid-image-template");
    imgsHolder = document.getElementById("grid-images");
    moreImgsBtn = document.getElementById("more-imgs-btn");
    loadingImgsMsgP = document.getElementById("loading-img-msg");
}

function loadImages(n)
{
    var len = loadedImgsIds.length;

    onLoadImgsStart();
    for (var i = len; i < n + len; i++) {
        requestStatuses[i] = false;
        loadNewImg(i);
    }
}

function loadImgsRequestsFinished()
{
    for (var i = 0; i < requestStatuses.length; i++)
        if (requestStatuses[i] == false)
            return false;
    return true;
}

function onLoadImgsStart()
{
    loadingImgsMsgP.innerHTML = "Loading...";
}

function onLoadImgsEnd()
{
    loadingImgsMsgP.innerHTML = "";
}