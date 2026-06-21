// js/ckeditor.ts
function loadCkEditorCss() {
  const id = "ckeditor5-blazor-css";
  const sourceEditorId = "ckeditor5-blazor-source-editor-css";
  const customId = "custom-ckeditor5-blazor-css";
  if (!document.getElementById(id)) {
    const link = document.createElement("link");
    link.id = id;
    link.rel = "stylesheet";
    link.href = "/_content/CkEditor5.Blazor/css/ckeditor5.css";
    document.head.appendChild(link);
  }
  if (!document.getElementById(sourceEditorId)) {
    const link = document.createElement("link");
    link.id = sourceEditorId;
    link.rel = "stylesheet";
    link.href = "/_content/CkEditor5.Blazor/css/ckeditor5-sourceediting-codemirror.css";
    document.head.appendChild(link);
  }
  if (!document.getElementById(customId)) {
    const customLink = document.createElement("link");
    customLink.id = customId;
    customLink.rel = "stylesheet";
    customLink.href = "/_content/Heracles.Blazor/css/ckeditor-custom.css";
    document.head.appendChild(customLink);
  }
}
export {
  loadCkEditorCss
};
