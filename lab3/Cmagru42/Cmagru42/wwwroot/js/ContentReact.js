function imgLike(element) {
    if (!isAuthenticated) {
        alert("You must login to like or comment.");
        return;
    }

    var gridEl = findAncestor(element, "grid-element");
    var data = {
        ContentId: gridEl.id,
    }

    console.log("Liking");
    $.ajax({
        type: "POST",
        url: "/PhotoWall/LikeImg",
        dataType: 'json',
        contentType: 'application/json',
        data: JSON.stringify(data),

        success: function(result) {
            if (result.success) {
                gridEl.querySelector(".likes-count").innerHTML = result.currentLikes;
            } else {
                alert("Could not like...");
            }
        },
        error: function(xhr, textStatus, error) {
        }

    });
}

function findAncestor (el, cls) {
    while ((el = el.parentElement) && !el.classList.contains(cls));
    return el;
}