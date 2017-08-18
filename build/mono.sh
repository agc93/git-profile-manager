#!/usr/bin/env bash
if ! [ -x "$(command -v mono)" ]; then
  echo 'Error: mono is not installed.' >&2
  if [ "$EUID" -ne 0 ]
    then echo "Please run as root"
    exit 401
  fi
  apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
  echo "deb http://download.mono-project.com/repo/ubuntu xenial main" | tee /etc/apt/sources.list.d/mono-official.list
  apt-get update && apt-get install -y mono-runtime mono-mcs $@
fi