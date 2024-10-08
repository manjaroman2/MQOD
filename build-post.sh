#!/usr/bin/bash

projpath="/home/marc/RiderProjects/MQOD"
#projassetpath="/home/marc/dev/URPTest3D"
projassetpath="/home/marc/dev/MQODAssets"
gamedir="/home/marc/RiderProjects/Death Must Die"

# --- change this if you know what you're doing  

game_name="Death Must Die"
name="MQOD"
source="${projpath}/MQOD/bin"
asset="${projassetpath}/Assets/AssetBundles/moreqodassets"
mods="${gamedir}/Mods"
gameid="2334730" 

moddir="${mods}/${name}"
mkdir -p "${moddir}"
dll="${name}.dll"
deps="UniverseLib.Mono.dll"
usage() {
    echo "Options: "
    echo "Debug"
    echo "Release"
    echo "ReleaseZip"
}
install() {
    if [ -n "${asset}" ]; then 
        echo "rm ${moddir}/${name}Assets"
        rm "${moddir}/${name}Assets" 2> /dev/null 
        cp "${asset}" "${moddir}/${name}Assets" 
        echo "${asset} -> ${moddir}/${name}Assets"
    fi 
    
    echo "rm ${mods}/${dll}"
    rm "${mods}/${dll}"
    cp "${source}" "${mods}/${dll}"
    echo "${source} -> ${mods}/${dll}"
}

if [ -z "$1" ]; then
    usage
    exit 1
fi

if [ "$1" = "Debug" ]; then
    source="${source}/Debug/${dll}"
else
    if [ "$1" = "Release" ]; then
        source="${source}/Release/${dll}"
    else
        if [ "$1" = "ReleaseZip" ]; then
            source="${source}/Release/${dll}"
            install
            targetzip="${name}.zip"
	    cd "${gamedir}"
            rm "${targetzip}" 2> /dev/null
            zip -9 -r "${targetzip}" "Mods/${name}.dll" "UserLibs/${deps}" "Mods/${name}"
            exit 0
        else
            echo "$1"
            usage
            exit 1
        fi
    fi
fi

pkill -HUP "${game_name}" 
install
steam steam://rungameid/${gameid}
exit 0 
