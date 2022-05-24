(function () {
    window.QuillFunctions = {
        createQuill: function (quillElement, toolbarId, placeholder) {
            var options = {
                debug: 'error',
                modules: {
                    toolbar: toolbarId
                },
                placeholder: placeholder,
                readOnly: false,
                theme: 'snow'
            };
            // set quill at the object we can call
            // methods on later
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
                content = JSON.parse(quillContent);
                return quillControl.__quill.setContents(content, 'api');
            } catch (err) {
                console.error(err);
                return;
            }
        },
    };
})();
//Reference https://blazorhelpwebsite.com/ViewBlogPost/12