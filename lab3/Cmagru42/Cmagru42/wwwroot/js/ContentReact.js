function imgLike(element) {
    if (!isAuthenticated) {
        alert("You must login to like or comment.");
        return;
    }

    var gridEl = findAncestor(element, "grid-element");
    var data = {
        ContentId: gridEl.id,
    }

    if (element.disabled)
        return;

    element.disabled = true;
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

            element.disabled = false;
        },
        error: function(xhr, textStatus, error) {
            element.disabled = false;
        }

    });
}