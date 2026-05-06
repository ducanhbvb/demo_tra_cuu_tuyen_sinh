// Khởi tạo cấu hình dùng chung cho TinyMCE
var CmsEditor = {
    allowedPasteTags: new Set([
        'P', 'BR', 'STRONG', 'B', 'EM', 'I', 'U', 'UL', 'OL', 'LI', 'BLOCKQUOTE',
        'H2', 'H3', 'H4', 'A', 'IMG', 'FIGURE', 'FIGCAPTION', 'TABLE', 'THEAD', 'TBODY',
        'TR', 'TH', 'TD'
    ]),

    allowedPasteClasses: new Set(['image', 'align-left', 'align-right', 'align-center', 'align-justify']),

    // Làm sạch HTML khi copy từ Word/Google Docs/web/PDF: bỏ font/style rác nhưng giữ định dạng cơ bản.
    cleanRichHtml: function (html) {
        if (!html) return '';

        var template = document.createElement('template');
        template.innerHTML = html
            .replace(/<!--[\s\S]*?-->/g, '')
            .replace(/<\/?o:[^>]*>/gi, '')
            .replace(/<\/?w:[^>]*>/gi, '')
            .replace(/<\/?m:[^>]*>/gi, '')
            .replace(/<xml[\s\S]*?<\/xml>/gi, '');

        this.cleanNode(template.content);
        return template.innerHTML;
    },

    wrapTableCells: function (html) {
        return (html || '')
            .replace(/<(td|th)(\b[^>]*)>([\s\S]*?)<\/\1>/gi, function (match, tag, attrs, inner) {
                if (/^\s*<(p|ul|ol|blockquote|h2|h3|h4|table)\b/i.test(inner)) return match;
                return '<' + tag + attrs + '><p>' + inner + '</p></' + tag + '>';
            });
    },

    cleanNode: function (root) {
        var disallowed = root.querySelectorAll('script,style,iframe,object,embed,form,input,button,textarea,select,option,meta,link,base,font');
        disallowed.forEach(function (node) { node.remove(); });

        var nodes = Array.prototype.slice.call(root.querySelectorAll('*'));
        for (var i = 0; i < nodes.length; i++) {
            var node = nodes[i];
            var tagName = node.tagName;

            this.normalizeWordTag(node);

            if (!this.allowedPasteTags.has(tagName)) {
                this.unwrapNode(node);
                continue;
            }

            this.cleanAttributes(node);
        }
    },

    normalizeWordTag: function (node) {
        var style = (node.getAttribute('style') || '').toLowerCase();
        if (!style) return;

        if (/font-weight\s*:\s*(bold|[7-9]00)/.test(style) && node.tagName !== 'STRONG' && node.tagName !== 'B') {
            node.innerHTML = '<strong>' + node.innerHTML + '</strong>';
        }
        if (/font-style\s*:\s*italic/.test(style) && node.tagName !== 'EM' && node.tagName !== 'I') {
            node.innerHTML = '<em>' + node.innerHTML + '</em>';
        }
        if (/text-decoration[^;]*underline/.test(style) && node.tagName !== 'U') {
            node.innerHTML = '<u>' + node.innerHTML + '</u>';
        }
    },

    cleanAttributes: function (node) {
        var attrs = Array.prototype.slice.call(node.attributes);
        for (var i = 0; i < attrs.length; i++) {
            var name = attrs[i].name.toLowerCase();
            var value = attrs[i].value;

            if (name.indexOf('on') === 0 || name === 'style' || name === 'srcdoc' || name.indexOf('data-') === 0) {
                node.removeAttribute(attrs[i].name);
                continue;
            }

            if (!this.isAllowedAttribute(node.tagName, name, value)) {
                node.removeAttribute(attrs[i].name);
            }
        }

        if (node.hasAttribute('class')) {
            var keptClasses = this.filterAllowedClasses(node.getAttribute('class'));
            if (keptClasses.length > 0) node.setAttribute('class', keptClasses.join(' '));
            else node.removeAttribute('class');
        }

        if (node.tagName === 'A' && node.getAttribute('target') === '_blank') {
            node.setAttribute('rel', 'noopener noreferrer');
        }
    },

    isAllowedAttribute: function (tagName, name, value) {
        if (name === 'class') {
            return this.filterAllowedClasses(value).length > 0;
        }

        if (tagName === 'A') return ['href', 'title', 'target', 'rel'].indexOf(name) >= 0;
        if (tagName === 'IMG') return ['src', 'alt', 'title', 'width', 'height'].indexOf(name) >= 0;
        if (name === 'title') return true;

        return false;
    },

    filterAllowedClasses: function (value) {
        var allowed = this.allowedPasteClasses;
        return (value || '')
            .split(/\s+/)
            .filter(function (cls) { return allowed.has(cls); });
    },

    unwrapNode: function (node) {
        var parent = node.parentNode;
        if (!parent) return;
        while (node.firstChild) parent.insertBefore(node.firstChild, node);
        parent.removeChild(node);
    },

    // Khởi tạo TinyMCE trên phần tử (khuyên dùng thẻ <textarea id="..."></textarea>)
    init: function (targetId) {
        tinymce.init({
            selector: '#' + targetId,
            base_url: '/lib/tinymce',
            suffix: '.min',
            height: 400,
            menubar: false,
            plugins: 'image link lists table wordcount fullscreen code preview',
            toolbar: 'undo redo | blocks | bold italic underline | alignleft aligncenter alignright alignjustify | bullist numlist | link image table | cleancontent removeformat | fullscreen preview code',
            
            // Cấu hình ảnh
            image_caption: true, // Native support <figure> và <figcaption>
            image_title: true,
            automatic_uploads: true,
            file_picker_types: 'image',
            convert_urls: false, // Dừng TinyMCE tự động chuyển đổi URL tương đối thành tuyệt đối (hoặc ngược lại) làm lỗi src ảnh

            // Đồng bộ whitelist với HtmlSanitizerHelper phía server để tránh editor tạo HTML sẽ bị loại bỏ khi lưu/render.
            valid_elements: 'p[class],br,strong/b,em/i,u,ul,ol,li,blockquote,h2,h3,h4,' +
                'a[href|title|target=_blank|rel],' +
                'img[src|alt|title|width|height|class],' +
                'figure[class],figcaption,table[class],thead,tbody,tr,th[class],td[class]',
            invalid_elements: 'script,style,iframe,object,embed,form,input,button,textarea,select,option,meta,link,base',
            extended_valid_elements: 'a[href|title|target|rel],img[src|alt|title|width|height|class],figure[class],table[class],th[class],td[class]',
            valid_classes: {
                '*': 'image align-left align-right align-center align-justify'
            },
            link_default_target: '_blank',
            link_default_protocol: 'https',

            // Paste: giữ định dạng cơ bản nhưng bỏ font-family/font-size/color/style rác từ Word/Google Docs/web/PDF.
            paste_as_text: false,
            paste_remove_styles_if_webkit: true,
            paste_webkit_styles: 'none',
            paste_merge_formats: true,
            paste_data_images: false,
            paste_preprocess: function (plugin, args) {
                args.content = CmsEditor.cleanRichHtml(args.content);
            },
            paste_postprocess: function (plugin, args) {
                CmsEditor.cleanNode(args.node);
            },

            forced_root_block: 'p',
            formats: {
                alignleft: { selector: 'p,h2,h3,h4,blockquote,figure,table', classes: 'align-left' },
                aligncenter: { selector: 'p,h2,h3,h4,blockquote,figure,table', classes: 'align-center' },
                alignright: { selector: 'p,h2,h3,h4,blockquote,figure,table', classes: 'align-right' },
                alignjustify: { selector: 'p,h2,h3,h4,blockquote', classes: 'align-justify' }
            },

            setup: function (editor) {
                editor.ui.registry.addButton('cleancontent', {
                    text: 'Làm sạch font',
                    tooltip: 'Bỏ font, cỡ chữ, màu chữ và style rác; giữ định dạng cơ bản',
                    onAction: function () {
                        var selected = editor.selection.getContent({ format: 'html' });
                        if (selected) {
                            editor.selection.setContent(CmsEditor.wrapTableCells(CmsEditor.cleanRichHtml(selected)));
                        } else {
                            editor.setContent(CmsEditor.wrapTableCells(CmsEditor.cleanRichHtml(editor.getContent())));
                        }
                    }
                });

                editor.on('PastePreProcess', function (e) {
                    e.content = CmsEditor.cleanRichHtml(e.content);
                });

                editor.on('PastePostProcess', function (e) {
                    CmsEditor.cleanNode(e.node);
                });
            },

            // Style trong khung soạn thảo: ép font chuẩn, không phụ thuộc font copy từ nguồn ngoài.
            content_style: `
                body { font-family: 'Be Vietnam Pro', -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol"; font-size: 15px; line-height: 1.7; }
                .align-left { text-align: left; }
                .align-center { text-align: center; }
                .align-right { text-align: right; }
                .align-justify { text-align: justify; }
                figure.image { margin: 15px auto; text-align: center; }
                figure.image img { display: inline-block; max-width: 100%; height: auto; border-radius: 6px; }
                figure.image figcaption { font-size: 0.85em; color: #64748b; font-style: italic; margin-top: 5px; }
                figure.align-left { float: left; margin-top: 0; margin-right: 20px; margin-bottom: 10px; }
                figure.align-right { float: right; margin-top: 0; margin-left: 20px; margin-bottom: 10px; }
            `,

            // API Upload Ảnh
            images_upload_handler: function (blobInfo, progress) {
                return new Promise((resolve, reject) => {
                    var xhr = new XMLHttpRequest();
                    // Đổi đường dẫn phù hợp với Backend của tool hiện tại
                    xhr.open('POST', '/Handlers/UploadImage.ashx');

                    xhr.upload.onprogress = function (e) {
                        progress(e.loaded / e.total * 100);
                    };

                    xhr.onload = function() {
                        if (xhr.status < 200 || xhr.status >= 300) {
                            reject('HTTP Error: ' + xhr.status);
                            return;
                        }

                        var json = JSON.parse(xhr.responseText);

                        if (!json || typeof json.url !== 'string') {
                            reject('Invalid JSON: ' + xhr.responseText);
                            return;
                        }

                        // Trả về url để hiển thị trên TinyMCE
                        resolve(json.url);
                    };

                    xhr.onerror = function () {
                        reject('Image upload failed due to a XHR Transport error. Code: ' + xhr.status);
                    };

                    var formData = new FormData();
                    // Chú ý: Backend đang đọc Request.Files["image"] nên key phải là 'image'
                    formData.append('image', blobInfo.blob(), blobInfo.filename());

                    xhr.send(formData);
                });
            }
        });
    },

    // Lấy giá trị nội dung
    getContent: function (targetId) {
        var editor = tinymce.get(targetId);
        return editor ? editor.getContent() : document.getElementById(targetId).value;
    },

    // Cập nhật giá trị vào element (thường là để set dữ liệu cũ lúc mở form Sửa)
    setContent: function (targetId, html) {
        var editor = tinymce.get(targetId);
        if (editor) {
            editor.setContent(this.wrapTableCells(this.cleanRichHtml(html || '')));
        } else {
            // Trong trường hợp TinyMCE chưa kịp render, set value cho thẻ textarea
            document.getElementById(targetId).value = this.wrapTableCells(this.cleanRichHtml(html || ''));
        }
    },

    // Xóa nội dung
    clear: function (targetId) {
        this.setContent(targetId, '');
    },

    // Đồng bộ nội dung lấy html chuẩn để submit (lấy từ TinyMCE sang hfNoiDung)
    syncToHiddenField: function(targetId, hfClientId) {
        var editor = tinymce.get(targetId);
        var html = editor ? this.wrapTableCells(this.cleanRichHtml(editor.getContent())) : '';
        document.getElementById(hfClientId).value = html;
        return true; // Để OnClientClick return true
    },

    // Làm sạch toàn bộ nội dung editor trước khi submit form WebForms.
    syncAllEditors: function () {
        if (!window.tinymce) return true;
        tinymce.triggerSave();
        tinymce.editors.forEach(function (editor) {
            var cleaned = CmsEditor.wrapTableCells(CmsEditor.cleanRichHtml(editor.getContent()));
            editor.setContent(cleaned);
            editor.save();
        });
        return true;
    }
};
