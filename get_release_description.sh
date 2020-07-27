read -r -d '' release_description << 'EOF'
The SAFE Authenticator acts as a gateway to the [SAFE Network](https://safenetwork.tech/) by enabling users to create an account & authenticate themselves onto the SAFE Network.
It helps users ensure they have full control over the permissions they grant to SAFE apps.

## Changelog
CHANGELOG_CONTENT

## SHA-256 checksums:
```
APK checksum
APK_CHECKSUM

IPA checksum
IPA_CHECKSUM
```

## Related Links
* SAFE Browser - [Desktop](https://github.com/maidsafe/safe_browser/releases/) | [Mobile](https://github.com/maidsafe/safe-mobile-browser/)
* [SAFE CLI](https://github.com/maidsafe/safe-api/tree/master/safe-cli)
* [SAFE Vault](https://github.com/maidsafe/safe_vault/releases/latest/)
* [safe_app_csharp](https://github.com/maidsafe/safe_app_csharp/)
EOF

apk_checksum=$(sha256sum "../net.maidsafe.SafeAuthenticator.apk" | awk '{ print $1 }')
ipa_checksum=$(sha256sum "../SafeAuthenticatoriOS.ipa" | awk '{ print $1 }')
changelog_content=$(sed '1,/]/d;/##/,$d' ../CHANGELOG.MD)
release_description=$(sed "s/APK_CHECKSUM/$apk_checksum/g" <<< "$release_description")
release_description=$(sed "s/IPA_CHECKSUM/$ipa_checksum/g" <<< "$release_description")

echo "${release_description/CHANGELOG_CONTENT/$changelog_content}" > release_description.txt
