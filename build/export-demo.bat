@echo off
cd ..
git archive --format=zip HEAD:NetMtsSdkDemo -o build\MtsSdkDemoProject.zip -9
