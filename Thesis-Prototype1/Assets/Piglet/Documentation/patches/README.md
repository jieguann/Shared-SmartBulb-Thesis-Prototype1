This is a patch to highlight text in the manifest.json listings for
KtxUnity/DracoUnity using HTML5 `<mark>` tags.

I wanted to use `pandoc-emphasize-code` for this purpose, but it only
works with an older versions of pandoc (pandoc-1.19), whereas the
`pandoc-crossref` filter I'm already using requires pandoc-2.11.2.

I may update the code from `pandoc-emphasize-code` myself at some point,
but I didn't want to get distracted from the Piglet 1.3.0 release
that I was working on at the time.
