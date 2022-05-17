(function () {
    window.QuillFunctions = {
        createQuill: function (quillElement, toolbarId, placeholder) {
            var options = {
                debug: 'info',
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
        }
    };
})();