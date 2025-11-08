window.loadQuill = function () {
    // Nếu đã có quill instance cũ thì hủy trước
    if (window.quill) {
        window.quill = null;
    }

    const container = document.getElementById("editor-container");
    if (!container) return;

    window.quill = new Quill(container, {
        theme: "snow",
        placeholder: "Nhập nội dung bài viết...",
        modules: {
            toolbar: [
                ["bold", "italic", "underline"],
                [{ list: "ordered" }, { list: "bullet" }],
                ["link", "image"],
                [{ align: [] }],
                ["clean"],
            ],
        },
    });
};

window.setQuillHtml = function (html) {
    if (window.quill && html) {
        const delta = window.quill.clipboard.convert(html);
        window.quill.setContents(delta, "silent");
    }
};

window.getQuillHtml = function () {
    if (!window.quill) return "";
    return window.quill.root.innerHTML;
};

window.destroyQuill = function () {
    if (window.quill) {
        window.quill = null;
    }
};