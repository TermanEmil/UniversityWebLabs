var commentContentEl = document.getElementById("add-comment-content");
var commentStatus = document.getElementById("add-comment-status");
var targetImgIdHolder = document.getElementById("target-img-id");
var commentTemplate = document.getElementById("img-comment");
var commentsContainer = document.getElementById("comments-container");

function addComment() {
    if (!isAuthenticated) {
        alert("You must login to like or comment.");
        return;
    }

    var data = {
        Content: commentContentEl.value,
        ImgId: targetImgIdHolder.value
    }
    commentStatus.innerHTML = "Sending...";

    $.ajax({
        type: "POST",
        url: "/PhotoWall/PostComment",
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(data),

        success: function(result) {
            if (result.success) {
                commentStatus.innerHTML = "Comment sent";
                commentContentEl.value = "";
                clearAllComments();
                loadAllComments(targetImgIdHolder.value);

            } else {
                commentStatus.innerHTML = result.error;
            }
        },
        error: function(xhr, textStatus, error) {
            commentStatus.innerHTML = error;
        }
    });
}

function loadAllComments(imgId) {
    var data = {
        ImgId: imgId
    }

    $.ajax({
        type: "POST",
        url: "/PhotoWall/GetComments",
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(data),

        success: function(result) {
            if (result.success) {
                result.allComments.forEach(function(comment) {
                   var commentEl = commentTemplate.content.cloneNode(true);
                   commentEl.querySelector(".user-name-field").innerHTML = comment.user + ":";
                   commentEl.querySelector(".comment-content").innerHTML = comment.content;

                   var time = comment.time.replace(comment.time.match(/\..*$/g), "");
                   time = time.replace("T", " ");
                   commentEl.querySelector(".time-field").innerHTML = time;

                   commentsContainer.appendChild(commentEl);
                });
            } else {
            }
        },
        error: function(xhr, textStatus, error) {
        }
    });
}

function clearAllComments() {
    while (commentsContainer.firstChild)
        commentsContainer.removeChild(commentsContainer.firstChild);
}