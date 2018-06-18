<template id="bet">
   <v-container fluid>
            <v-dialog v-model="dialog" max-width="350">
                <v-card>
                    <v-card-title class="headline">Cảnh báo!</v-card-title>
                    <v-card-text>Livefeed tỉ lệ đã tắt, có thể tỉ lệ ko phải là mới nhất. Bạn hãy cân nhắc cẩn thận.</v-card-text>
                    <v-card-actions>
                    <v-spacer></v-spacer>
                    <v-btn color="red darken-1" flat @click.native="dialog = false; confirm = true;">Chơi luôn</v-btn>
                    <v-btn color="green darken-1" @click.native="dialog = false;">Nghỉ</v-btn>
                    </v-card-actions>
                </v-card>
            </v-dialog>
            <v-layout class="mb-2" justify-center align-center row wrap>
                <v-flex d-flex lg2 md2 sm4 xs5>
                    <v-switch
                        v-model="showAll"
                        label="Show all"
                        color="green lighten-2"
                        hide-details></v-switch>
                </v-flex>
                <v-flex d-flex lg2 md2 sm4 xs5>
                    <span v-if="livefeedError">Live: OFF</span>
                    <span v-else>
                        Live: ON
                    </span>
                    <spinner v-show="showSpinner" width="5px" height="10px"></spinner>
                </v-flex>
            </v-layout>
            <v-layout justify-center row wrap>
                <template v-for="game in gameList">
                    <v-slide-x-transition :key="game.Id">
                        <match-bet :game.sync="game"
                            :minBet="minBet"
                            :maxBet="maxBet"
                            :step="step"
                            :confirm="confirm"
                            :now="now"
                            @error="emitError"
                            @success="emitSuccess"
                            @bet="userBet"/>
                    </v-slide-x-transition>
                </template>
            </v-layout>
    </v-container>
</template>
<script>
import Spinner from 'vue-spinner/src/ScaleLoader.vue'
import matchBet from './MatchBet.vue'
import axios from 'axios'
import API from '../API'
export default {
    name: 'Bet',
    template: '#bet',
    created: async function(){
        await this.init();
        //Subscribe to feed
        if(this.livefeed) {
            // console.log('Listen to feed..');
            this.livefeed.on("Fixtures", this.livefeedCallback);
        }
    },
    beforeDestroy: function(){
        if(this.livefeed) {
            // console.log('Stop listening to feed..');
            this.livefeed.off("Fixtures", this.livefeedCallback);
        }
    },
    components: {
        'match-bet': matchBet,
        'spinner' : Spinner
    },
    data: function () {
        return {
            maxBet: 0,
            minBet: 0,
            now: null,
            step: 0,
            games: [],
            dialog: false,
            confirm: true,
            showAll: false
        }
    },
    watch:{
        livefeedError(error){
            //If no livefeed then confirm needed
            if(error)
                this.confirm = false;
        }
    },
    computed: {
        gameList(){
            if(this.showAll)
                return this.games;
            return this.games.filter(g => !g.Expired);
        },
        livefeed(){
            return this.$store.getters.livefeed;
        },
        livefeedError(){
            return this.$store.getters.livefeedError;
        },
        showSpinner(){
            return !this.livefeedError;
        }
    },
    methods: {
        init: async function(){
            try {
                var { data } = await axios.get(API.BetVM);
                this.games = data.Games;
                this.maxBet = data.MaxBet;
                this.minBet = data.MinBet;
                this.now = data.Now;
                this.step = data.Step;
            } catch (error) {
                // Alert error...
                this.emitError("Ko tải được dữ liệu :(");
            }
        },
        userBet(bet) {
            //Warn if making bet when livefeed is off
            if(!this.confirm){
                this.dialog = true;
            }
                
        },
        emitError: function(mess){
            this.$emit("error", mess);
        },
        emitSuccess: function(mess){
            this.$emit("success", mess);
        },
        livefeedCallback: function(games){
            // console.log(games);
            //Update data
            games.forEach(g => {
                var game = this.games.find(game => game.Id == g.Id);
                if(game){
                    game.Odds1 = g.Odds1;
                    game.Odds2 = g.Odds2;
                    game.Win1 = g.Win1;
                    game.Win2 = g.Win2;
                }
            });
        }
    }
}
</script>
<style scoped>

</style>