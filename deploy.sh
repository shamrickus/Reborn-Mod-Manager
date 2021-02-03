#!/bin/bash

projs=( "RMM" )
contains=( "true" "false" )

for proj in "${projs[@]}"
do
    for contain in "${contains[@]}"
    do
        str=""
        if [ $contain == "true" ] ;
        then
            str="-Big"
        fi
        
        dotnet restore  "./$proj/$proj.csproj"
        dotnet publish -r linux-x64 -c Release -o "bin/linux/$proj$str" --self-contained $contain "./$proj/$proj.csproj"
        rm -rf "bin/linux/$proj$str/VPK/windows/"
        echo "Built, Compressing..."
        tar -czvf "bin/linux/$proj$str.tar.gz" "bin/linux/$proj$str"
    done
done
