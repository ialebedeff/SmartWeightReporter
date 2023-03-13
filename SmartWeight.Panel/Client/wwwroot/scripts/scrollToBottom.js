export function scrollSmoothlyToBottom(id) {
    const element = $(`#${id}`);
    element.animate({
        scrollTop: element.prop("scrollHeight")
    }, 500);
}