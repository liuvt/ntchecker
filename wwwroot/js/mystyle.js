window.isDesktopView = false; // Biến trạng thái để kiểm tra chế độ hiện tại

window.forceDesktopView = function () {
    if (!window.isDesktopView) {
        // Kích hoạt chế độ Desktop
        document.body.style.width = "1200px";
        document.body.style.minWidth = "1200px";
        document.body.style.overflowX = "auto";
        document.body.style.margin = "0 auto"; // Căn giữa nội dung

        let metaTag = document.querySelector("meta[name=viewport]");
        if (metaTag) {
            metaTag.setAttribute("content", "width=1200");
        } else {
            metaTag = document.createElement("meta");
            metaTag.name = "viewport";
            metaTag.content = "width=1200";
            document.head.appendChild(metaTag);
        }

    } else {
        // Hủy chế độ Desktop (Quay lại mặc định)
        document.body.style.width = "";
        document.body.style.minWidth = "";
        document.body.style.overflowX = "";
        document.body.style.margin = ""; // Hủy căn giữa khi tắt chế độ desktop

        let metaTag = document.querySelector("meta[name=viewport]");
        if (metaTag) {
            metaTag.setAttribute("content", "width=device-width, initial-scale=1");
        }

    }

    // Đảo trạng thái
    window.isDesktopView = !window.isDesktopView;
};