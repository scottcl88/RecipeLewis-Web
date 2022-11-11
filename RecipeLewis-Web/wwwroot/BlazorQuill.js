(function () {
    window.QuillFunctions = {
        createQuill: function (quillElement, toolbarId, placeholder) {
            let options = {
                debug: 'error',
                modules: {
                    toolbar: toolbarId
                },
                placeholder: placeholder,
                readOnly: false,
                theme: 'snow'
            };
            new Quill(quillElement, options);
        },
        getQuillContent: function (quillControl) {
            return JSON.stringify(quillControl.__quill.getContents());
        },
        getQuillHTML: function (quillControl) {
            return quillControl.__quill.root.innerHTML;
        },
        loadQuillContent: function (quillControl, quillContent) {
            try {
                let content = JSON.parse(quillContent);
                return quillControl.__quill.setContents(content, 'api');
            } catch (err) {
                console.error(err);
                return;
            }
        },
        loadDarkModeCss: function () {
            if (document.head.querySelector("link[href='css/quill_darkmode.css']") == null)
            {
                let link = document.createElement('link');
                link.type = 'text/css';
                link.rel = 'stylesheet';
                link.title = 'quillDarkMode';
                link.href = "css/quill_darkmode.css";

                document.head.appendChild(link);
            }
        },
        loadLightModeCss: function () {
            if (document.head.querySelector("link[href='css/quill_darkmode.css']") != null)
            {
                document.head.querySelector("link[href='css/quill_darkmode.css']").remove();
            }
        }
    };
})();
//Reference https://blazorhelpwebsite.com/ViewBlogPost/12