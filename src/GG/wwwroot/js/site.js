
const animatedEyes = document.getElementById('blinkingEyes');
console.log(animatedEyes);
function restartGIF(){
    animatedEyes.src = animatedEyes.getAttribute('src');
}
const interval = setInterval( () => {
    restartGIF()
}, 5000);


$(document).ready(function () {
    $('.custom-file-input').on("change", function () {
        var fileName = $(this).val().split("\\").pop();
        $(this).next('.custom-file-label').html(fileName);
    });
});
