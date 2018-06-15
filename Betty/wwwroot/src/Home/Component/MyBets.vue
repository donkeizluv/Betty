<template id="bet">
   <v-container fluid>
            <v-layout justify-center row wrap>
               <v-flex lg12>
                   <span>All my bets...</span>
               </v-flex>
            </v-layout>
    </v-container>
</template>
<script>
import axios from 'axios'
import API from '../API'
export default {
    name: 'MyBets',
    template: '#bet',
    created: async function(){
        
    },
    components: {
    },
    data: function () {
        return {
        }
    },
    computed: {
        
    },
    methods: {
    }
}
</script>
<style scoped>

</style>