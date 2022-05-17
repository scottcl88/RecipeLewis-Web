(function () {
    window.QuillFunctions = {
        createQuill: function (quillElement, toolbarId) {
            var options = {
                debug: 'info',
                modules: {
                    toolbar: toolbarId
                },
                placeholder: 'Compose an epic...',
                readOnly: false,
                theme: 'snow'
            };
            // set quill at the object we can call
            // methods on later
            new Quill(quillElement, options);
        }
    };
})();