//script for initializing the Quill editor
//Quill rich text editor is from https://quilljs.com/
//initialization code is based on the full editor initialization in the demo
//Quill can also be found on GitHub here: https://github.com/quilljs/quill
(function () {
    $(document).ready(function () {
        //var bindings = {
        //    // This will overwrite the default binding also named 'tab'
        //    tab: {
        //        key: 9,
        //        handler: function () {
        //            // Handle tab
        //        }
        //    },
        //    disablefocus: {
        //        key: 'M',
        //        shortKey: true,
        //        handler: function () {

        //        }
        //    }
        //};
        var fonts = ['arial', 'bruno-ace', 'caveat', 'comic', 'courier', 'creepster', 'impact', 'la-belle-aurore', 'spectral-sc', 'quintessential', 'times'];
        var Font = Quill.import('formats/font');
        Font.whitelist = fonts;
        Quill.register(Font, true);

        var editor = new Quill('#editorContainer .editor', {
            bounds: '#editorContainer .editor',
            modules: {
                'formula': false,
                'syntax': true,
                'toolbar': [
                    [{ 'font': fonts }, { 'size': [] }],
                    ['bold', 'italic', 'underline', 'strike'],
                    [{ 'color': [] }, { 'background': [] }],
                    [{ 'script': 'super' }, { 'script': 'sub' }],
                    [{ 'header': '1' }, { 'header': '2' }, 'blockquote', 'code-block'],
                    [{ 'list': 'ordered' }, { 'list': 'bullet' }, { 'indent': '-1' }, { 'indent': '+1' }],
                    [{ 'align': [] }],
                    ['link', 'image'],
                    ['clean']
                ],
                keyboard: {
                    bindings: bindings
                }
            },
            placeholder: 'Write your own destiny...',
            name: 'EditorContent',
            theme: 'snow'
        });

        //editor.keyboard.addBinding({
        //    key: 'M',
        //    shortKey: true
        //}, function());

        loadFonts();
    });



    //keyboard-friendly way to progress to the next element
    $("#editor div.ql-editor").on("keydown", function (event) {
        if ((event.key === "Tab" && event.ctrlKey) || (event.key === "Tab" && event.altKey)) {
            $("div.slide1 div.buttonGroup button.nextPage").focus();
        }
    });

    //function for loading the fonts from the Google fonts api
    //This is not being used in the current version of the editor, but is here for later use if that changes
    function loadFonts() {
        window.WebFontConfig = {
            google: { families: ['Inconsolata::latin', 'Ubuntu+Mono::latin', 'Slabo+27px::latin', 'Roboto+Slab::latin'] }
        };
        (function () {
            var wf = document.createElement('script');
            wf.src = 'https://ajax.googleapis.com/ajax/libs/webfont/1/webfont.js';
            wf.type = 'text/javascript';
            wf.async = 'true';
            var s = document.getElementsByTagName('script')[0];
            s.parentNode.insertBefore(wf, s);
        })();
    }
})();