var video;
var webcamStream;
var canvas, ctx;

navigator.getUserMedia = (
    navigator.getUserMedia ||
    navigator.webkitGetUserMedia ||
    navigator.mozGetUserMedia ||
    navigator.msGetUserMedia);

function initWebCam()
{
    // Get the canvas and obtain a context for
    // drawing in it
    canvas = document.getElementById("snapshotCanvas");
    ctx = canvas.getContext('2d');

    document.getElementById("webcamOnOffBtn").textContent = "Turn webcam On";
}

function startWebcam()
{
    if (navigator.getUserMedia)
    {
        navigator.getUserMedia(
            // constraints
            { video: true, audio: false },

            // successCallback
            function(localMediaStream)
            {
                video = document.querySelector('video');
                video.srcObject = localMediaStream;
                webcamStream = localMediaStream.getTracks()[0];

                var btn = document.getElementById("webcamOnOffBtn");
                btn.textContent = "Turn webcam Off";

            },

            // errorCallback
            function(err)
            {
                console.log("The following error occured: " + err);
            });
    }
    else
    {
        console.log("getUserMedia not supported");
    }
}
    
function stopWebcam()
{
    if (webcamStream != undefined)
        webcamStream.stop();
}

function snapshot()
{
    if (webcamStream == undefined)
    {
        ctx.clearRect(0, 0, canvas.width, canvas.height);
        return;
    }
    
    var ratio = webcamStream.getSettings().aspectRatio;
    var width = canvas.width;
    var height = canvas.width / ratio;
    var topPadding = (canvas.height - height) / 2;
    
    ctx.drawImage(video, 0, topPadding, width, height);
}

function webcamToggle()
{
    if (webcamStream == undefined)
    {
        startWebcam();
    }

    if (webcamStream)
    {
        stopWebcam();

        var btn = document.getElementById("webcamOnOffBtn");

        btn.textContent = "Turn webcam On";
        webcamStream = undefined;
    }
}

function uploadCurrentSnapshot() {
    var statusLog = document.getElementById("upload-current-snapshot-status");
    statusLog.innerHTML = "Uploading...";

    var data = { RawImg: canvas.toDataURL() };
    $.ajax({
        type: 'POST',
        url: '/Photoroom/UploadImgRaw',
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function(result) {
            if (result.success) {
                statusLog.innerHTML = "Successfully uploaded!";
            } else {
                statusLog.innerHTML = "Couldn't upload...";
            }
        }
    });
}

function overlayImg(img) {
    var snapshotCanvas = document.getElementById("snapshotCanvas");
    var ctx = snapshotCanvas.getContext('2d');
    ctx.drawImage(img, 0, 0);
}

function createSideImg(rawImg, parent) {
    var img = new Image();
    img.classList.add("side-img");
    img.src = rawImg;

    img.onclick = function() {
        overlayImg(this);
    };
    parent.appendChild(img);
}

