$content = [System.IO.File]::ReadAllText("./readme.md")
$result = $([System.Text.RegularExpressions.Regex]::Replace($content, "<!-- HIDE -->(.|\n|\r)*<!-- END:HIDE -->",""))
$result = $([System.Text.RegularExpressions.Regex]::Replace($result, "<!-- REPLACE (.*) -->(.)*<!-- END:REPLACE -->",{ $args[0].Groups[1] }))
[System.IO.File]::WriteAllText("./docs/index.md", $result)
