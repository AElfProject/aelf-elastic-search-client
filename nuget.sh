#!/usr/bin/env bash
set -ev

TAG=$1
MYGET_API_KEY=$2
VERSION=`echo ${TAG} | cut -b 2-`

rm -r node/*
src_path=src/
for path in ${src_path};
do
    cd ${path}
    for name in `ls -lh | grep ^d | grep AElf | grep -v Tests | awk '{print $NF}'`;
    do
        if [[ -f ${name}/${name}.csproj ]] && [[ 1 -eq $(grep -c "GeneratePackageOnBuild"  ${name}/${name}.csproj) ]];then
            dotnet build /clp:ErrorsOnly ${name}/${name}.csproj --configuration Debug -P:Version=${VERSION} -P:Authors=AElf -o ../node
        fi
    done
    cd ../node
    for name in `ls *.nupkg`;
    do
        echo ${name}
        dotnet nuget push ${name} -k ${MYGET_API_KEY} -s https://www.myget.org/F/aelf-project-dev/api/v3/index.json
    done
    cd ../
done
