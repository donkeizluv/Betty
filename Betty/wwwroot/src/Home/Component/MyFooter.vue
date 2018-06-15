<template>
    <v-footer height="auto">
        <v-card flat
                tile
                class="indigo lighten-1 white--text text-xs-center">
            <v-card-text>
                <v-btn v-for="icon in icons"
                       :key="icon"
                       icon
                       class="mx-3 white--text">
                    <v-icon size="24px">{{ icon }}</v-icon>
                </v-btn>
            </v-card-text>
            <v-card-text class="white--text pt-0">
                Bla bla blaBla bla blaBla bla blaBla bla blaBla bla blaBla bla bla
            </v-card-text>
            <v-card-text class="white--text">
                &copy;2018 — <strong>HDSS</strong>
            </v-card-text>
        </v-card>
    </v-footer>
</template>

<script>
  export default {
    data: () => ({
      icons: ['fab fa-facebook']
    })
  }
</script>