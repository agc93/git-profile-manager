FROM library/centos:7

RUN groupadd -r app -g 1000 && useradd -u 1000 -r -g app -m -d /home/app -s /sbin/nologin -c "App user" app && \
    chmod 755 /home/app

RUN yum install -y sudo && \
    echo "app ALL=(root) NOPASSWD:ALL" > /etc/sudoers.d/app && \
    chmod 0440 /etc/sudoers.d/app

RUN yum install -y libicu libunwind git

ADD ./packages/rpm-x64/git-profile-manager*.rpm /tmp/

RUN rpm -i /tmp/git-profile-manager*.rpm && rm -f /tmp/git-profile-manager*.rpm

USER app

CMD bash