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
        alert("Please start the webcam");
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
