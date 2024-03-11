const actionButton = $("#actionbutton");
const inputUrl = $("#inputurl");
const output = $("#output");
const outputPanel = $("#outputpanel");
const outputInfo = $("#outputinfo");


function SendRequestToShortenUrl() {

    const formData = {
        url: inputUrl.val(),
    };
    $.ajax({
        type: "POST",
        url: "/Api/TryAdd",
        data: formData,
        dataType: "json",
        error: function (xhr, status, error) {
            outputInfo.html("Something went wrong");
            outputPanel.show();
            output.val("");
        },
        success: function (response) {
            if (response.success) {
                outputInfo.html(`Success: ${response.shortenedUrl}; ${response.timeToExpireInSec}`);
                output.val(`${window.location.origin}/${response.shortenedUrl}`);
            }
            else {
                outputInfo.html("Something went wrong");
                output.val("");
            }
            outputPanel.show();
        }
    });
}

outputPanel.hide();
actionButton.click(SendRequestToShortenUrl);