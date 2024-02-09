param (
    $Version
)

docker build --rm -t loremfoobar/resharper-inspections-bitbucket-pipe:$Version .
