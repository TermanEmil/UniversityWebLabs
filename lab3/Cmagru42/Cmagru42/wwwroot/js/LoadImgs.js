var loadedImgsIds = [ ];
var imgTemplate;
var imgsHolder;
var deltaImgsToLoad = 5;
var moreImgsBtn;
var loadingImgsMsgP;

function loadNewImgs(requiredImgs) {
    var data = {
        PhotosIds: loadedImgsIds,
        RequiredImgs: requiredImgs
    };

    var requestHolder = document.createElement('div');
    imgsHolder.appendChild(requestHolder);

    $.ajax({
        type: 'POST',
        url: '/PhotoWall/GetNewImgs',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(data),

        success: function(result) {
            if (result.success) {
                for (var i = 0; i < result.imgs.length; i++) {
                    var responseImg = result.imgs[i];
                    var img = imgTemplate.content.cloneNode(true);

                    img.querySelector(".grid-image").src = responseImg.imgBase64;
                    img.querySelector(".likes-count").innerHTML = responseImg.likes;
                    img.querySelector(".comments-count").innerHTML = responseImg.comments;
                    img.querySelector(".grid-element").id = responseImg.imgId;
                    img.id = responseImg.imgId;

                    requestHolder.appendChild(img);
                    loadedImgsIds.push(responseImg.imgId);
                }
            } else {
                imgsHolder.removeChild(requestHolder);
            }

            onLoadImgsEnd();
        },
        error: function(xhr, textStatus, error) {
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

    var imgNbs = [];
    for (var i = len; i < n + len; i++) {
        imgNbs.push(i);
    }

    onLoadImgsStart();
    loadNewImgs(imgNbs);
}

function onLoadImgsStart()
{
    loadingImgsMsgP.innerHTML = "Loading...";
}

function onLoadImgsEnd()
{
    loadingImgsMsgP.innerHTML = "";
}